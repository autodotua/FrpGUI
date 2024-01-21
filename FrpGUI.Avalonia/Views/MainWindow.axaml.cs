using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using System.Linq;

namespace FrpGUI.Avalonia.Views;

public partial class MainWindow : Window
{
    private readonly bool startup;

    public MainWindow(bool startup)
    {
        InitializeComponent();
        this.startup = startup;
    }
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Config.AppConfig.Instance.Save();
        base.OnClosing(e);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (startup)
        {
            Hide();
        }
    }

    private void Window_Closing(object sender, WindowClosingEventArgs e)
    {
        MainView mainView = Content as MainView;
        MainViewModel mainViewModel = mainView?.DataContext as MainViewModel;

        if (mainViewModel != null && mainViewModel.FrpConfigs.Any(p => p.ProcessStatus == ProcessStatus.Running))
        {
            e.Cancel = true;
            var trayIcon = TrayIcon.GetIcons(App.Current)[0];
            trayIcon.IsVisible = true;
            Hide();
        }

    }
}
