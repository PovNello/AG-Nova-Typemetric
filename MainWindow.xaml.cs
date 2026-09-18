using System.Windows;
using System.Windows.Input;

namespace Typemetric
{
    public partial class MainWindow : Window
    {
        private Core.MetricsTracker _tracker;
        private Core.DatabaseManager _db;

        public MainWindow(Core.MetricsTracker tracker, Core.DatabaseManager db)
        {
            InitializeComponent();
            _tracker = tracker;
            _db = db;

            this.IsVisibleChanged += (s, e) => {
                if (this.IsVisible) RefreshData();
            };
        }

        private void RefreshData()
        {
            if (_tracker == null || _db == null || RadioToday == null) return;
            
            _tracker.FlushNow();
            
            bool isAllTime = RadioAllTime.IsChecked == true;
            string date = System.DateTime.Now.ToString("yyyy-MM-dd");

            System.Collections.Generic.Dictionary<string, int> keyFreqs;
            System.Collections.Generic.Dictionary<string, int> mouseFreqs;

            if (isAllTime)
            {
                var stats = _db.GetAllTimeTotals();
                KeysTextBlock.Text = stats.Keystrokes.ToString("N0");
                MouseTextBlock.Text = stats.MouseClicks.ToString("N0");
                ScrollTextBlock.Text = stats.ScrollTicks.ToString("N0");

                keyFreqs = _db.GetAllTimeFrequencies();
                mouseFreqs = _db.GetAllTimeMouseFrequencies();
            }
            else
            {
                var stats = _db.GetTodayTotals(date);
                KeysTextBlock.Text = stats.Keystrokes.ToString("N0");
                MouseTextBlock.Text = stats.MouseClicks.ToString("N0");
                ScrollTextBlock.Text = stats.ScrollTicks.ToString("N0");

                keyFreqs = _db.GetFrequenciesForDate(date);
                mouseFreqs = _db.GetMouseFrequenciesForDate(date);
            }

            HeatmapKeyboard.UpdateHeatmap(keyFreqs);
            HeatmapMouse.UpdateHeatmap(mouseFreqs);
        }

        private void Timeframe_Changed(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            App.TrimMemory();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}