using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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
        }

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            IPHostEntry host = null;
            await Task.Run(() =>
            {
                host = Dns.GetHostEntry(Dns.GetHostName());
            });
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPs.Add(ip.ToString());
                }
            }
        }

        public Config Config => Config.Instance;
        public ObservableCollection<string> IPs { get; } = new ObservableCollection<string>();
        protected override Button CheckButton => btnCheck;
        protected override Button RestartButton => btnRestart;

        protected override Button StartButton => btnStart;

        protected override Button StopButton => btnStop;

        public override Task StopAsync()
        {
            return base.StopAsync();
        }

        protected override void UpdateUI()
        {
            base.UpdateUI();
            Dispatcher.Invoke(() =>
            {
                gbxInfo.IsEnabled = FrpConfig.ProcessStatus == ProcessStatus.Running;
            });
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

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            gbxInfo.IsEnabled = false;
            Dictionary<string, object> dictionary = null;
            await Task.Run(async () =>
            {
                var info = await HttpHelper.Instance.GetServerInfoAsync(FrpConfig as ServerConfig);
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
                items = await HttpHelper.Instance.GetProxiesAsync(FrpConfig as ServerConfig, type);
            });
            data.ItemsSource = items;
            gbxInfo.IsEnabled = true;
        }
    }
}