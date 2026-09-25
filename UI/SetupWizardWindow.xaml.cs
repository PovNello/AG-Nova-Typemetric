using System.IO;
using System.Windows;
using Microsoft.Win32;
using Typemetric.Core;

namespace Typemetric.UI
{
    public partial class SetupWizardWindow : Window
    {
        public string SelectedDbPath { get; private set; }

        public SetupWizardWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Select Database Location",
                FileName = "metrics.db",
                Filter = "SQLite Database (*.db)|*.db",
                DefaultExt = ".db"
            };

            if (dialog.ShowDialog() == true)
            {
                PathTextBox.Text = dialog.FileName;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                MessageBox.Show("Please select a path first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Step1Grid.Visibility = Visibility.Collapsed;
            Step2Grid.Visibility = Visibility.Visible;
        }

        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedDbPath = PathTextBox.Text;
            bool runAtStartup = StartupCheckBox.IsChecked == true;

            var config = new AppConfig { DbPath = SelectedDbPath, RunAtStartup = runAtStartup };
            if (!config.Save())
            {
                MessageBox.Show("Unable to save configuration in the application directory. Please run the application as Administrator, or move the application to a user folder (like Documents or Desktop).", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (runAtStartup)
            {
                StartupManager.EnableRunAtStartup();
            }
            else
            {
                StartupManager.DisableRunAtStartup();
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
