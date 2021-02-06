using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(bool autoStart = false)
        {
            bool clientOn = Config.Instance.ClientOn;
            bool serverOn = Config.Instance.ServerOn;
            InitializeComponent();

            ProcessHelper.Output += ProcessHelper_Output;
            if (FzLib.Program.Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist)
            {
                btnStartup.Content = "√开机自启";
            }
            if (autoStart)
            {
                if (clientOn)
                {
                    client.StartAsync();
                }
                if (serverOn)
                {
                    server.StartAsync();
                }
            }
        }

        private void ProcessHelper_Output(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                lbxLogs.Items.Add(e.Data);
                lbxLogs.ScrollIntoView(lbxLogs.Items[^1]);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (server.ProcessStatus != ProcessStatus.NotRun || client.ProcessStatus != ProcessStatus.NotRun)
            {
                e.Cancel = true;
                await ShowMessage("请先关闭所有执行中的程序");
            }
        }

        public async Task ShowMessage(string message)
        {
            tbkDialogMessage.Text = message;
            await dialog.ShowAsync();
        }

        private void TitleBarButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).ShowTray();
            Visibility = Visibility.Collapsed;
        }

        private void TitleBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (FzLib.Program.Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist)
            {
                btnStartup.Content = "×开机自启";
                (App.Current as App).SetStartup(false);
            }
            else
            {
                btnStartup.Content = "√开机自启";
                (App.Current as App).SetStartup(true);
            }
        }
    }
}