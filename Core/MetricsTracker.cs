using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Typemetric.Core
{
    public class MetricsTracker
    {
        private readonly DatabaseManager _db;
        private readonly GlobalHooks _hooks;

        private Dictionary<string, int> _keyCounts = new Dictionary<string, int>();
        private Dictionary<string, int> _mouseCounts = new Dictionary<string, int>();
        private int _scrollTicks = 0;

        private readonly object _lock = new object();
        private bool _isRunning = false;

        public MetricsTracker(DatabaseManager db, GlobalHooks hooks)
        {
            _db = db;
            _hooks = hooks;
        }

        public void Start()
        {
            _hooks.KeyPressed += OnKeyPressed;
            _hooks.MouseClicked += OnMouseClicked;
            _hooks.MouseScrolled += OnMouseScrolled;
            _hooks.Start();
            
            _isRunning = true;
            Task.Run(FlushLoop);
        }

        public void Stop()
        {
            _isRunning = false;
            _hooks.Stop();
            _hooks.KeyPressed -= OnKeyPressed;
            _hooks.MouseClicked -= OnMouseClicked;
            _hooks.MouseScrolled -= OnMouseScrolled;
            FlushToDatabase();
        }

        private void OnKeyPressed(string key)
        {
            lock (_lock)
            {
                if (_keyCounts.ContainsKey(key)) _keyCounts[key]++;
                else _keyCounts[key] = 1;
            }
        }

        private void OnMouseClicked(string button)
        {
            lock (_lock)
            {
                if (_mouseCounts.ContainsKey(button)) _mouseCounts[button]++;
                else _mouseCounts[button] = 1;
            }
        }

        private void OnMouseScrolled(int delta)
        {
            lock (_lock)
            {
                _scrollTicks += delta;
            }
        }

        private async Task FlushLoop()
        {
            while (_isRunning)
            {
                await Task.Delay(TimeSpan.FromMinutes(1)); // Flush every minute
                FlushToDatabase();
            }
        }

        public void FlushNow()
        {
            FlushToDatabase();
        }

        private void FlushToDatabase()
        {
            Dictionary<string, int> keysToFlush;
            Dictionary<string, int> mouseToFlush;
            int scrollsToFlush;

            lock (_lock)
            {
                keysToFlush = new Dictionary<string, int>(_keyCounts);
                mouseToFlush = new Dictionary<string, int>(_mouseCounts);
                scrollsToFlush = _scrollTicks;

                _keyCounts.Clear();
                _mouseCounts.Clear();
                _scrollTicks = 0;
            }

            if (keysToFlush.Count == 0 && mouseToFlush.Count == 0 && scrollsToFlush == 0)
                return;

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            if (keysToFlush.Count > 0) _db.IncrementKeyCounts(date, keysToFlush);
            if (mouseToFlush.Count > 0) _db.IncrementMouseCounts(date, mouseToFlush);
            if (scrollsToFlush != 0) _db.IncrementScrollTicks(date, scrollsToFlush);
        }
    }
}
