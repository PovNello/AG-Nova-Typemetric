using System;
using System.IO;
using System.Windows;
using Typemetric.Core;

namespace Typemetric
{
    public partial class App : Application
    {
        private GlobalHooks _hooks;
        private DatabaseManager _db;
        private MetricsTracker _tracker;
        private UI.TrayIconManager _trayIcon;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var config = AppConfig.Load();

            // Recovery check
            if (!string.IsNullOrEmpty(config.DbPath) && !File.Exists(config.DbPath))
            {
                string backupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metrics_backup.db");
                if (File.Exists(backupPath))
                {
                    var result = MessageBox.Show("The primary database was not found at the previous location. Do you want to restore it from the local backup?", "Database Not Found", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        var dialog = new Microsoft.Win32.SaveFileDialog
                        {
                            Title = "Select New Database Location for Recovery",
                            FileName = "metrics.db",
                            Filter = "SQLite Database (*.db)|*.db",
                            DefaultExt = ".db"
                        };
                        
                        if (dialog.ShowDialog() == true)
                        {
                            File.Copy(backupPath, dialog.FileName, true);
                            config.DbPath = dialog.FileName;
                            if (!config.Save())
                            {
                                MessageBox.Show("Unable to save configuration in the application directory. Please run the application as Administrator, or move the application to a user folder.", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                                Application.Current.Shutdown();
                                return;
                            }
                        }
                        else
                        {
                            Application.Current.Shutdown();
                            return;
                        }
                    }
                    else
                    {
                        config.DbPath = null;
                    }
                }
                else
                {
                    config.DbPath = null;
                }
            }

            if (string.IsNullOrEmpty(config.DbPath))
            {
                var wizard = new UI.SetupWizardWindow();
                bool? result = wizard.ShowDialog();
                
                if (result != true || string.IsNullOrEmpty(wizard.SelectedDbPath))
                {
                    Application.Current.Shutdown();
                    return;
                }
                // config is saved within the wizard now, so we reload it
                config = AppConfig.Load();
            }

            _hooks = new GlobalHooks();
            _db = new DatabaseManager(config.DbPath);
            _tracker = new MetricsTracker(_db, _hooks);
            _tracker.Start();

            _trayIcon = new UI.TrayIconManager(_tracker, _db);
            
            // Trim memory footprint immediately after startup
            TrimMemory();

            // Ensure Run at Startup is aligned with config
            if (config.RunAtStartup)
                StartupManager.EnableRunAtStartup();
            else
                StartupManager.DisableRunAtStartup();
        }

        public static void TrimMemory()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            if (System.Environment.OSVersion.Platform == System.PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(System.IntPtr proc, int min, int max);

        private void OnExit(object sender, ExitEventArgs e)
        {
            _tracker?.Stop();
            _hooks?.Dispose();
            _trayIcon?.Dispose();
        }
    }
}
