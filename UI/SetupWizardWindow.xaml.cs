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

        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                MessageBox.Show("Please select a path first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedDbPath = PathTextBox.Text;
            var config = new AppConfig { DbPath = SelectedDbPath };
            config.Save();

            this.DialogResult = true;
            this.Close();
        }
    }
}
