using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FrpGUI
{
    public abstract class PanelBase : UserControl
    {
        protected abstract ProcessHelper Process { get; }
        protected abstract Button StartButton { get; }
        protected abstract Button StopButton { get; }
        protected abstract Button RestartButton { get; }
        protected abstract string Type { get; }
        protected abstract IToIni ConfigItem { get; }
        public ProcessStatus ProcessStatus { get; private set; }

        public PanelBase()
        {
            DataContext = this;
            Process.Exited += Process_Exited;
            Initialized += PanelBase_Initialized;
        }

        private void PanelBase_Initialized(object sender, EventArgs e)
        {
            ChangeStatus(ProcessStatus.NotRun);
            StartButton.Click += btnStart_Click;
            RestartButton.Click += btnRestart_Click;
            StopButton.Click += btnStop_Click;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ChangeStatus(ProcessStatus.NotRun);
            });
        }

        protected virtual void ChangeStatus(ProcessStatus status)
        {
            Debug.Assert(Dispatcher.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId);
            ProcessStatus = status;
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = false;
            RestartButton.IsEnabled = false;
            switch (status)
            {
                case ProcessStatus.NotRun:
                    StartButton.IsEnabled = true;
                    break;

                case ProcessStatus.Running:
                    StopButton.IsEnabled = true;
                    RestartButton.IsEnabled = true;
                    break;

                case ProcessStatus.Busy:
                    break;

                default:
                    break;
            }
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            await StartAsync();
        }

        public virtual async Task StartAsync()
        {
            ChangeStatus(ProcessStatus.Busy);
            await Process.StartAsync(Type, ConfigItem);
            ChangeStatus(ProcessStatus.Running);
            Config.Instance.Save();
        }

        private async void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus(ProcessStatus.Busy);
            await Process.RestartAsync();
            ChangeStatus(ProcessStatus.Running);
            Config.Instance.Save();
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            await StopAsync();
        }

        public virtual async Task StopAsync()
        {
            ChangeStatus(ProcessStatus.Busy);
            await Process.StopAsync();
            ChangeStatus(ProcessStatus.NotRun);
        }

        private void OpenUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    System.Diagnostics.Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    System.Diagnostics.Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    System.Diagnostics.Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}