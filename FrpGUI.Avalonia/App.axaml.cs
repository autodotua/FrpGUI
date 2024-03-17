using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using FrpGUI.Util;
using FzLib.Avalonia.Dialogs;
using System;
using System.Linq;

namespace FrpGUI.Avalonia;

public partial class App : Application
{
    internal HttpServerHelper HttpServerHelper { get; } = new HttpServerHelper();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            bool startup = desktop.Args.Length > 0 && desktop.Args[0] == "s";
            desktop.MainWindow = new MainWindow(startup);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView();
        }

        base.OnFrameworkInitializationCompleted();

        if (AppConfig.Instance.RemoteControlEnable)
        {
            HttpServerHelper.StartAsync().ConfigureAwait(false);
        }
    }

    private void OpenMenuItem_Click(object sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow.Show();
            TrayIcon.GetIcons(this)[0].IsVisible = false;
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }
    private async void ExitMenuItem_Click(object sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainViewModel mainViewModel = (desktop.MainWindow as MainWindow).GetDataContext();
            if (mainViewModel != null && mainViewModel.FrpConfigs.Any(p => p.ProcessStatus == ProcessStatus.Running))
            {
                desktop.MainWindow.Show();
                TrayIcon.GetIcons(this)[0].IsVisible = false;
                int count = mainViewModel.FrpConfigs.Where(p => p.ProcessStatus == ProcessStatus.Running).Count();
                if (await desktop.MainWindow.ShowYesNoDialogAsync("退出", $"存在{count}个正在运行的frp进程，是否退出？") == true)
                {
                    foreach (var frp in mainViewModel.FrpConfigs)
                    {
                        await frp.StopAsync();
                    }
                    desktop.MainWindow.Close();
                }
            }
            else
            {
                desktop.MainWindow.Close();
            }
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    private void TrayIcon_Clicked(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow.Show();
            TrayIcon.GetIcons(this)[0].IsVisible = false;
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }
}
