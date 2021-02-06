using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
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
    /// <summary>
    /// ServerPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ServerPanel : PanelBase
    {
        public ServerPanel()
        {
            InitializeComponent();
        }

        protected override void ChangeStatus(ProcessStatus status)
        {
            base.ChangeStatus(status);
            btnOpenDashBoard.IsEnabled = status == ProcessStatus.Running;
            if (status == ProcessStatus.Running)
            {
                Config.Instance.ServerOn = true;
            }
            else
            {
                Config.Instance.ServerOn = false;
            }
        }

        public ServerConfig Server => Config.Instance.Server;

        protected override Button StartButton => btnStart;
        protected override Button StopButton => btnStop;
        protected override Button RestartButton => btnRestart;
        protected override string Type => "s";
        protected override IToIni ConfigItem => Server;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("http://localhost:" + Server.DashBoardPort);
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