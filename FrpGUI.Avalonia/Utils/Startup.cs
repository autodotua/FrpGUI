using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace FrpGUI.Avalonia.Utils
{
    public static class Startup
    {
        static Startup()
        {
            AppName = Process.GetCurrentProcess().ProcessName;
            Common = false;
        }

        public static string AppName { get; set; }
        private static bool common;
        public static bool Common
        {
            get => common;
            set
            {
                if (!Common)
                {
                    startupFolderFilePath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + AppName + ".lnk";

                    registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                }
                else
                {
                    startupFolderFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + "\\" + AppName + ".lnk";

                    registryKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                }
                common = value;
            }
        }

        public static string SourceFileName { get; set; } = Process.GetCurrentProcess().MainModule.FileName;
        private static string startupFolderFilePath;
        private static RegistryKey registryKey = null;



        public static void DeleteStartupFolderShortcut()
        {
            if (File.Exists(startupFolderFilePath))
            {
                File.Delete(startupFolderFilePath);
            }
        }

        public static void CreateRegistryKey(string arguments = null)
        {
            string value = "\"" + SourceFileName + "\"";
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                value += " " + arguments;
            }
            registryKey.SetValue(AppName, value);

        }

        public static bool IsRegistryKeyExist()
        {
            if (!(registryKey.GetValue(AppName) is string registryValue))
            {
                return false;
            }
            else
            {
                return registryValue.StartsWith("\"" + SourceFileName) || registryValue.StartsWith(SourceFileName);
            }
        }
        public static void DeleteRegistryKey()
        {
            registryKey.DeleteValue(AppName);
        }
    }
}

