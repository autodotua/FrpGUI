using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FrpGUI
{
    public abstract class PanelBase : UserControl
    {
        protected ProcessHelper process = new ProcessHelper();
        protected abstract Button StartButton { get; }
        protected abstract Button StopButton { get; }
        protected abstract Button RestartButton { get; }
        protected abstract string Type { get; }
        protected abstract IToIni ConfigItem { get; }
        public ProcessStatus ProcessStatus { get; private set; }

        public PanelBase()
        {
            DataContext = this;
            process.Exited += Process_Exited;
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
            ChangeStatus(ProcessStatus.NotRun);
        }

        protected virtual void ChangeStatus(ProcessStatus status)
        {
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

        public async Task StartAsync()
        {
            ChangeStatus(ProcessStatus.Busy);
            await process.StartAsync(Type, ConfigItem);
            ChangeStatus(ProcessStatus.Running);
            Config.Instance.Save();
        }

        private async void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus(ProcessStatus.Busy);
            await process.RestartAsync();
            ChangeStatus(ProcessStatus.Running);
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus(ProcessStatus.Busy);
            await process.StopAsync();
            ChangeStatus(ProcessStatus.NotRun);
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}