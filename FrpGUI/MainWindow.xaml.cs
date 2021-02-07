using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Regex rLog = new Regex(@"(?<Time>[0-9/: ]{19}) \[(?<Type>.)\] \[[^\]]+\] (?<Content>.*)", RegexOptions.Compiled);

        public MainWindow(bool autoStart = false)
        {
            bool clientOn = Config.Instance.ClientOn;
            bool serverOn = Config.Instance.ServerOn;
            InitializeComponent();
            DataContext = this;

            ProcessHelper.Output += ProcessHelper_Output;
            if (FzLib.Program.Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist)
            {
                menuStartup.IsChecked = true;
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

        public ObservableCollection<Log> Logs { get; } = new ObservableCollection<Log>();

        private void ProcessHelper_Output(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (rLog.IsMatch(e.Data))
                {
                    var match = rLog.Match(e.Data);
                    string time = match.Groups["Time"].Value;
                    string content = match.Groups["Content"].Value;
                    string type = match.Groups["Type"].Value;
                    Brush brush = Foreground;
                    if (type == "W")
                    {
                        brush = Brushes.Yellow;
                    }
                    else if (type == "E")
                    {
                        brush = Brushes.Red;
                    }
                    Logs.Add(new Log()
                    {
                        Time = time,
                        Content = content,
                        TypeBrush = brush
                    });
                }
                else
                {
                }
                lbxLogs.ScrollIntoView(Logs[^1]);
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
            Logs.Clear();
        }
    }

    public class Log
    {
        public string Time { get; set; }
        public string Content { get; set; }
        public Brush TypeBrush { get; set; }
    }
}