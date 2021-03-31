using FzLib.Basic;
using FzLib.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration.Internal;
using System.Diagnostics;
using System.Globalization;
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
        private PanelBase panel;

        public PanelBase Panel
        {
            get => panel;
            set => this.SetValueAndNotify(ref panel, value, nameof(Panel));
        }

        public ObservableCollection<FrpConfigBase> FrpConfigs { get; } = new ObservableCollection<FrpConfigBase>();

        private FrpConfigBase selectedFrpConfig;
        private ServerPanel serverPanel = new ServerPanel();
        private ClientPanel clientPanel = new ClientPanel();

        public FrpConfigBase SelectedFrpConfig
        {
            get => selectedFrpConfig;
            set
            {
                this.SetValueAndNotify(ref selectedFrpConfig, value, nameof(SelectedFrpConfig));
                if (value == null)
                {
                    Panel = null;
                }
                else if (value is ServerConfig s)
                {
                    serverPanel.SetConfig(s);
                    Panel = serverPanel;
                }
                else if (value is ClientConfig c)
                {
                    clientPanel.SetConfig(c);
                    Panel = clientPanel;
                }
            }
        }

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
            InitializeComponent();
            DataContext = ViewModel;

            ProcessHelper.Output += ProcessHelper_Output;
            if (FzLib.Program.Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist)
            {
                menuStartup.IsChecked = true;
            }
            foreach (var config in Config.Instance.FrpConfigs)
            {
                ViewModel.FrpConfigs.Add(config);
            }
            foreach (var config in from c in Config.Instance.FrpConfigs where c.AutoStart select c)
            {
                config.Start();
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

        private bool forceClose = false;

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (forceClose)
            {
                return;
            }
            SaveConfig();
            if (ViewModel.FrpConfigs.Any(p => p.ProcessStatus != ProcessStatus.NotRun))
            {
                e.Cancel = true;
                if (await ShowYesNoMessage("需要关闭所有执行中的FRP进程才可以关闭本程序，是否关闭？"))
                {
                    WindowState = WindowState.Minimized;
                    foreach (var config in ViewModel.FrpConfigs.Where(p => p.ProcessStatus != ProcessStatus.NotRun))
                    {
                        await config.StopAsync();
                    }
                    forceClose = true;
                    Close();
                }
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
            forceClose = true;
            SaveConfig();
            List<Task> tasks = new List<Task>();
            foreach (var config in ViewModel.FrpConfigs.Where(p => p.ProcessStatus != ProcessStatus.NotRun))
            {
                tasks.Add(config.StopAsync());
            }
            await Task.WhenAll(tasks);
            FzLib.Program.App.Restart(App.Current.Shutdown);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedFrpConfig != null)
            {
                if (ViewModel.SelectedFrpConfig.ProcessStatus != ProcessStatus.NotRun)
                {
                    await ShowMessage("该配置正在运行，请先停止进程");
                    return;
                }
                ViewModel.FrpConfigs.Remove(ViewModel.SelectedFrpConfig);
                SaveConfig();
            }
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).Tag.Equals("1"))
            {
                var config = new ServerConfig();
                int i = 1;
                while (ViewModel.FrpConfigs.Any(p => p.Name == "服务端" + i))
                {
                    i++;
                }
                config.Name = "服务端" + i;
                int serverIndex = ViewModel.FrpConfigs.Any(p => p is ServerConfig) ? ViewModel.FrpConfigs.Where(p => p is ServerConfig).Count() : 0;
                ViewModel.FrpConfigs.Insert(serverIndex, config);
                ViewModel.SelectedFrpConfig = config;
            }
            else
            {
                var config = new ClientConfig();
                int i = 1;
                while (ViewModel.FrpConfigs.Any(p => p.Name == "客户端" + i))
                {
                    i++;
                }
                config.Name = "客户端" + i;
                ViewModel.FrpConfigs.Add(config);
                ViewModel.SelectedFrpConfig = config;
            }
            SaveConfig();
        }

        private void SaveConfig()
        {
            Config.Instance.FrpConfigs = ViewModel.FrpConfigs.ToList();

            Config.Instance.Save();
        }

        private void CloneButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedFrpConfig != null)
            {
                var newItem = ViewModel.SelectedFrpConfig.Clone() as FrpConfigBase;
                newItem.ChangeStatus(ProcessStatus.NotRun);
                newItem.Name += "（副本）";
                ViewModel.FrpConfigs.Insert(ViewModel.FrpConfigs.IndexOf(ViewModel.SelectedFrpConfig) + 1, newItem);
                ViewModel.SelectedFrpConfig = newItem;
                SaveConfig();
            }
        }
    }

    public class Log
    {
        public DateTime Time { get; set; }
        public string Content { get; set; }
        public Brush TypeBrush { get; set; }
    }

    public class ProcessStatus2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            ProcessStatus status = (ProcessStatus)value;
            if (status == ProcessStatus.NotRun)
            {
                return Brushes.Red;
            }
            return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}