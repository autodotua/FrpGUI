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
    public partial class ServerPanel : UserControl
    {
        private ProcessHelper process = new ProcessHelper();

        public ServerPanel()
        {
            InitializeComponent();
            DataContext = this;
            process.Exited += Process_Exited;

            SetUIEnable(false);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            SetUIEnable(false);
        }

        private void SetUIEnable(bool running)
        {
            Dispatcher.Invoke(() =>
            {
                btnStart.IsEnabled = !running;
                btnRestart.IsEnabled = running;
                btnStop.IsEnabled = running;
                btnOpenDashBoard.IsEnabled = running;
            });
        }

        public ServerConfig Server => Config.Instance.Server;

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            process.Start("s", Server);
            SetUIEnable(true);
            Config.Instance.Save();
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            process.Restart();
            SetUIEnable(true);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            process.Stop();
            SetUIEnable(false);
        }

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