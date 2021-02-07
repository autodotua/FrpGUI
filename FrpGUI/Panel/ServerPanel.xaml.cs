using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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
    /// <summary>
    /// ServerPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ServerPanel : PanelBase
    {
        public ServerPanel()
        {
            InitializeComponent();
            process.Exited += Process_Exited;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
        }

        public override Task StopAsync()
        {
            return base.StopAsync();
        }

        protected override void ChangeStatus(ProcessStatus status)
        {
            base.ChangeStatus(status);
            gbxInfo.IsEnabled = status == ProcessStatus.Running;
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
            OpenUrl("http://admin:admin@localhost:" + Server.DashBoardPort);
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

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            gbxInfo.IsEnabled = false;
            Dictionary<string, object> dictionary = null;
            await Task.Run(async () =>
            {
                var info = await HttpHelper.Instance.GetServerInfoAsync();
                dictionary = new Dictionary<string, object>();
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(info))
                {
                    object obj = propertyDescriptor.GetValue(info);
                    dictionary.Add(propertyDescriptor.Name, obj);
                }
            });
            data.ItemsSource = dictionary;
            gbxInfo.IsEnabled = true;
        }

        private async void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            gbxInfo.IsEnabled = false;
            List<object> items = null;
            string type = (sender as ModernWpf.Controls.AppBarButton).Label.ToString().ToLower();
            await Task.Run(async () =>
            {
                items = await HttpHelper.Instance.GetProxiesAsync(type);
            });
            data.ItemsSource = items;
            gbxInfo.IsEnabled = true;
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = e.Column.Header switch
            {
                "Key" => "属性名",
                "Value" => "属性值",
                "name" => "命名",
                "today_traffic_in" => "进入传入流量",
                "today_traffic_out" => "进入传出流量",
                "cur_conns" => "当前连接数",
                "last_start_time" => "上一次开始",
                "last_close_time" => "上一次关闭",
                "status" => "状态",
                _ => e.Column.Header
            };
        }
    }
}