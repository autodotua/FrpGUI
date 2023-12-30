using FrpGUI.Config;
using FzLib;
using ModernWpf.FzExtension.CommonDialog;
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

namespace FrpGUI.WPF
{
    public class Log : INotifyPropertyChanged
    {
        private string content;
        private DateTime time;
        private Brush typeBrush;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Content
        {
            get => content;
            set => this.SetValueAndNotify(ref content, value, nameof(Content));
        }

        public DateTime Time
        {
            get => time;
            set => this.SetValueAndNotify(ref time, value, nameof(Time));
        }
        public Brush TypeBrush
        {
            get => typeBrush;
            set => this.SetValueAndNotify(ref typeBrush, value, nameof(TypeBrush));
        }
    }

    public partial class MainWindow : Window
    {
        private bool forceClose = false;
        private Regex rFrpLog = new Regex(@"(?<Time>[0-9/: ]{19}) \[(?<Type>.)\] \[[^\]]+\] (?<Content>.*)", RegexOptions.Compiled);
        public MainWindow(bool autoStart = false)
        {
            InitializeComponent();
            Logger.NewLog += (s, e) => AddLogOnMainThread(e.Message, e.Type);
            DataContext = ViewModel;

            foreach (var config in AppConfig.Instance.FrpConfigs)
            {
                ViewModel.FrpConfigs.Add(config);
            }
            foreach (var config in from c in AppConfig.Instance.FrpConfigs where c.AutoStart select c)
            {
                config.Start();
            }
        }

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();
        public void AddLog(string message, char type)
        {
            if (type == 'O')
            {
                if (rFrpLog.IsMatch(message))
                {
                    var match = rFrpLog.Match(message);
                    message = match.Groups["Content"].Value;
                    type = match.Groups["Type"].Value[0];
                }
                else
                {
                    type = message.Contains("error") ? 'E' : 'I';
                }
            }

            Brush brush = Foreground;
            if (type == 'W')
            {
                brush = Brushes.Orange;
            }
            else if (type == 'E')
            {
                brush = Brushes.Red;
            }

            if (ViewModel.Logs.Count >= 2)
            {
                for (int i = 1; i < 2; i++)
                {
                    if (ViewModel.Logs[^i].Content == message)
                    {
                        ViewModel.Logs[^i].Time = DateTime.Now;
                        return;
                    }
                }
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

        public void AddLogOnMainThread(string message, char type)
        {
            Dispatcher.Invoke(() => AddLog(message, type));
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

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedFrpConfig != null)
            {
                if (ViewModel.SelectedFrpConfig.ProcessStatus != ProcessStatus.NotRun)
                {
                    await CommonDialog.ShowErrorDialogAsync("该配置正在运行，请先停止进程");
                    return;
                }
                ViewModel.FrpConfigs.Remove(ViewModel.SelectedFrpConfig);
                SaveConfig();
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Logs.Clear();
        }

        private async void RestartButton_Click(object sender, RoutedEventArgs e)
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

        private void SaveConfig()
        {
            AppConfig.Instance.FrpConfigs = ViewModel.FrpConfigs.ToList();

            AppConfig.Instance.Save();
        }

        private void TrayButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).ShowTray();
            Visibility = Visibility.Collapsed;
        }

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
                if (await CommonDialog.ShowYesNoDialogAsync("关闭", "需要关闭所有执行中的FRP进程才可以关闭本程序，是否关闭？"))
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ClientPanel clientPanel = new ClientPanel();
        private int maxLogCount = 10000;
        private PanelBase panel;
        private FrpConfigBase selectedFrpConfig;
        private ServerPanel serverPanel = new ServerPanel();
        public MainWindowViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FrpConfigBase> FrpConfigs { get; } = new ObservableCollection<FrpConfigBase>();
        public ObservableCollection<Log> Logs { get; } = new ObservableCollection<Log>();
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

        public PanelBase Panel
        {
            get => panel;
            set => this.SetValueAndNotify(ref panel, value, nameof(Panel));
        }

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

        public bool Startup
        {
            get => FzLib.Program.Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist;
            set => (App.Current as App).SetStartup(value);
        }
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