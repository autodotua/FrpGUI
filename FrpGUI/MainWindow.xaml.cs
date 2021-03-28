﻿using FzLib.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrpGUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Log> Logs { get; } = new ObservableCollection<Log>();

        private int maxLogCount = 10000;

        public MainWindowViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int MaxLogCount
        {
            get => maxLogCount;
            set
            {
                if (value > 100000)
                {
                    value = 100000;
                }
                else if (value < 10)
                {
                    value = 10;
                }
                this.SetValueAndNotify(ref maxLogCount, value, nameof(MaxLogCount));
            }
        }
    }

    public partial class MainWindow : Window
    {
        private Regex rLog = new Regex(@"(?<Time>[0-9/: ]{19}) \[(?<Type>.)\] \[[^\]]+\] (?<Content>.*)", RegexOptions.Compiled);
        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public MainWindow(bool autoStart = false)
        {
            bool clientOn = Config.Instance.ClientOn;
            bool serverOn = Config.Instance.ServerOn;
            InitializeComponent();
            DataContext = ViewModel;

            ProcessHelper.Output += ProcessHelper_Output;
            if (FzLib.Program.Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist)
            {
                menuStartup.IsChecked = true;
            }
            if (autoStart)
            {
                if (clientOn)
                {
                    client.Start();
                }
                if (serverOn)
                {
                    server.Start();
                }
            }
        }

        private void ProcessHelper_Output(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (rLog.IsMatch(e.Data))
            {
                var match = rLog.Match(e.Data);
                string content = match.Groups["Content"].Value;
                string type = match.Groups["Type"].Value;
                AddLogOnMainThread(content, type);
            }
            else
            {
                AddLogOnMainThread(e.Data, e.Data.Contains("error") ? "E" : "");
            }
        }

        public void AddLogOnMainThread(string message, string type)
        {
            Dispatcher.Invoke(() => AddLog(message, type));
        }

        public void AddLog(string message, string type)
        {
            Brush brush = Foreground;
            if (type == "W")
            {
                brush = Brushes.Yellow;
            }
            else if (type == "E")
            {
                brush = Brushes.Red;
            }
            ViewModel.Logs.Add(new Log()
            {
                Time = DateTime.Now,
                Content = message,
                TypeBrush = brush
            });
            if (ViewModel.Logs.Count > 0)
            {
                lbxLogs.ScrollIntoView(ViewModel.Logs[^1]);
                while (ViewModel.Logs.Count > ViewModel.MaxLogCount)
                {
                    ViewModel.Logs.RemoveAt(0);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (server.ProcessStatus != ProcessStatus.NotRun || client.ProcessStatus != ProcessStatus.NotRun)
            {
                e.Cancel = true;
                await ShowMessage("请先关闭所有执行中的程序");
            }
        }

        public async Task ShowMessage(string message)
        {
            dialog.PrimaryButtonText = "确定";
            dialog.SecondaryButtonText = null;
            tbkDialogMessage.Text = message;
            await dialog.ShowAsync();
        }

        public async Task<bool> ShowYesNoMessage(string message)
        {
            dialog.PrimaryButtonText = "是";
            dialog.SecondaryButtonText = "否";
            tbkDialogMessage.Text = message;
            if (await dialog.ShowAsync() == ModernWpf.Controls.ContentDialogResult.Primary)
            {
                return true;
            }
            return false;
        }

        private void MenuStartup_Click(object sender, RoutedEventArgs e)
        {
            if (FzLib.Program.Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist)
            {
                menuStartup.IsChecked = false;
                (App.Current as App).SetStartup(false);
            }
            else
            {
                menuStartup.IsChecked = true;
                (App.Current as App).SetStartup(true);
            }
        }

        private void MenuTray_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).ShowTray();
            Visibility = Visibility.Collapsed;
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var regFile = System.IO.Path.Combine(FzLib.Program.App.ProgramDirectoryPath, "RegistWebBrowse.reg");
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "cmd",
                    Arguments = $"/c regedit.exe \"{regFile}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                await ShowMessage("运行注册表注册文件失败：" + ex.Message);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Logs.Clear();
        }

        private async void MenuRestart_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.Save();
            if (server.ProcessStatus == ProcessStatus.Running)
            {
                await server.StopAsync();
            }
            if (client.ProcessStatus == ProcessStatus.Running)
            {
                await client.StopAsync();
            }
            FzLib.Program.App.Restart(App.Current.Shutdown);
        }
    }

    public class Log
    {
        public DateTime Time { get; set; }
        public string Content { get; set; }
        public Brush TypeBrush { get; set; }
    }
}