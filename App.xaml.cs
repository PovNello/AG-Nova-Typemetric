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

            if (string.IsNullOrEmpty(config.DbPath))
            {
                var wizard = new UI.SetupWizardWindow();
                bool? result = wizard.ShowDialog();
                
                if (result != true || string.IsNullOrEmpty(wizard.SelectedDbPath))
                {
                    Application.Current.Shutdown();
                    return;
                }
                config.DbPath = wizard.SelectedDbPath;
            }

            _hooks = new GlobalHooks();
            _db = new DatabaseManager(config.DbPath);
            _tracker = new MetricsTracker(_db, _hooks);
            _tracker.Start();

            _trayIcon = new UI.TrayIconManager(_tracker, _db);
            
            // Trim memory footprint immediately after startup
            TrimMemory();

            // Ensure Run at Startup is enabled
            StartupManager.EnableRunAtStartup();
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
