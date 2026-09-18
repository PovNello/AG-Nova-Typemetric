using Microsoft.Win32;
using System;
using System.Reflection;

namespace Typemetric.Core
{
    public static class StartupManager
    {
        private const string AppName = "AGNovaTypemetric";
        
        public static void EnableRunAtStartup()
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                string exePath = Environment.ProcessPath; // Gets the actual .exe when published
                
                if (!string.IsNullOrEmpty(exePath))
                {
                    key?.SetValue(AppName, $"\"{exePath}\"");
                }
            }
            catch { /* Log silently if needed */ }
        }

        public static void DisableRunAtStartup()
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                key?.DeleteValue(AppName, false);
            }
            catch { }
        }

        public static bool IsRunAtStartupEnabled()
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
                return key?.GetValue(AppName) != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
