using Microsoft.Win32;
using System;
using System.Reflection;
using System.IO;

namespace Typemetric.Core
{
    public static class StartupManager
    {
        private const string ShortcutName = "AGNovaTypemetric.lnk";

        private static string GetStartupFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ShortcutName);
        }

        public static void EnableRunAtStartup()
        {
            try
            {
                string shortcutPath = GetStartupFolderPath();
                string exePath = Environment.ProcessPath;

                if (!string.IsNullOrEmpty(exePath))
                {
                    Type t = Type.GetTypeFromProgID("WScript.Shell");
                    dynamic shell = Activator.CreateInstance(t);
                    dynamic shortcut = shell.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = exePath;
                    shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
                    shortcut.Description = "Typemetric Startup";
                    shortcut.Save();
                }
            }
            catch { /* Log silently if needed */ }
        }

        public static void DisableRunAtStartup()
        {
            try
            {
                string shortcutPath = GetStartupFolderPath();
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }
            }
            catch { }
        }

        public static bool IsRunAtStartupEnabled()
        {
            try
            {
                string shortcutPath = GetStartupFolderPath();
                return File.Exists(shortcutPath);
            }
            catch
            {
                return false;
            }
        }
    }
}
