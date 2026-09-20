using System;
using System.Windows;
using System.Windows.Controls;
using H.NotifyIcon;

namespace Typemetric.UI
{
    public class TrayIconManager : IDisposable
    {
        private TaskbarIcon _taskbarIcon;
        private MainWindow _mainWindow;
        private Core.MetricsTracker _tracker;
        private Core.DatabaseManager _db;

        public TrayIconManager(Core.MetricsTracker tracker, Core.DatabaseManager db)
        {
            _tracker = tracker;
            _db = db;

            var iconUri = new Uri("pack://application:,,,/Assets/icon.ico", UriKind.Absolute);
            using (var stream = Application.GetResourceStream(iconUri).Stream)
            {
                _taskbarIcon = new TaskbarIcon
                {
                    ToolTipText = "AG Nova Typemetric",
                    Icon = new System.Drawing.Icon(stream),
                    Visibility = Visibility.Visible
                };
            }
            _taskbarIcon.ForceCreate();

            ContextMenu menu = new ContextMenu();
            MenuItem showItem = new MenuItem { Header = "Show Metrics" };
            showItem.Click += (s, e) => ShowMainWindow();
            MenuItem exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (s, e) => Application.Current.Shutdown();

            menu.Items.Add(showItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(exitItem);

            _taskbarIcon.ContextMenu = menu;
            _taskbarIcon.TrayMouseDoubleClick += (s, e) => ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            if (_mainWindow == null || !_mainWindow.IsLoaded)
            {
                _mainWindow = new MainWindow(_tracker, _db);
                _mainWindow.Show();
            }
            else
            {
                _mainWindow.Show();
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.Activate();
            }
        }

        public void Dispose()
        {
            _taskbarIcon?.Dispose();
        }
    }
}
