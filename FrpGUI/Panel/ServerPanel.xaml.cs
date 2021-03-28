﻿using System;
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
        protected override ProcessHelper Process { get; } = ProcessHelper.Server;

        public ServerPanel()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPs.Add(ip.ToString());
                }
            }
            InitializeComponent();
            Process.Exited += Process_Exited;
            Process.Started += Process_Started;
        }

        public ObservableCollection<string> IPs { get; } = new ObservableCollection<string>();

        private void Process_Started(object sender, EventArgs e)
        {
            Dispatcher?.Invoke(() =>
            {
                ChangeStatus(ProcessStatus.Running);
            });
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Dispatcher?.Invoke(() =>
            {
                ChangeStatus(ProcessStatus.NotRun);
            });
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
        public Config Config => Config.Instance;

        protected override Button StartButton => btnStart;
        protected override Button StopButton => btnStop;
        protected override Button RestartButton => btnRestart;
        protected override Button CheckButton => btnCheck;
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