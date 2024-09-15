using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FzLib.Avalonia.Dialogs;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using Avalonia.Threading;
using Avalonia.Media;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FrpGUI.Enums;
using FrpGUI.Avalonia.DataProviders;
using static FrpGUI.Avalonia.Models.WebFrpProcess;
using FrpGUI.Configs;
using System.Text.Json;
using System.IO;
using FrpGUI.Avalonia.Models;

namespace FrpGUI.Avalonia;

public partial class App : Application
{
    public App()
    {

    }
    public static IServiceProvider Services { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        if (OperatingSystem.IsWindows())
        {
            Resources.Add("ContentControlThemeFontFamily", new FontFamily("Microsoft YaHei"));
        }
        else if (OperatingSystem.IsBrowser())
        {
            Resources.Add("ContentControlThemeFontFamily", new FontFamily("avares://FrpGUI.Avalonia/Assets#Microsoft YaHei"));
        }
        var builder = Host.CreateApplicationBuilder();
        var tempConfig = AppConfigBase<UIConfig>.Get();
        switch (tempConfig.RunningMode)
        {
            case RunningMode.Singleton:
                builder.Services.AddSingleton<IDataProvider, LocalDataProvider>();
                break;
            case RunningMode.Service:
                builder.Services.AddSingleton<IDataProvider, WebDataProvider>();
                break;
        }

        builder.Services.AddTransient<MainWindow>();
        builder.Services.AddTransient<MainView>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<ClientPanel>();
        builder.Services.AddTransient<ServerPanel>();
        builder.Services.AddTransient<FrpConfigViewModel>();

        builder.Services.AddTransient<RuleDialog>();
        builder.Services.AddTransient<RuleViewModel>();

        builder.Services.AddTransient<SettingsDialog>();
        builder.Services.AddTransient<SettingViewModel>();

        builder.Services.AddTransient<LogPanel>();
        builder.Services.AddTransient<LogViewModel>();

        builder.Services.AddSingleton(AppConfigBase<UIConfig>.Get());

        var host = builder.Build();
        Services = host.Services;
        host.Start();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime s)
        {
            s.MainView = new MainView();
        }


        base.OnFrameworkInitializationCompleted();

    }


    private void ExitMenuItem_Click(object sender, EventArgs e)
    {
        //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        //{
        //    MainViewModel mainViewModel = (desktop.MainWindow as MainWindow).GetDataContext();
        //    if (mainViewModel != null && mainViewModel.FrpConfigs.Any(p => p.ProcessStatus == ProcessStatus.Running))
        //    {
        //        desktop.MainWindow.Show();
        //        TrayIcon.GetIcons(this)[0].IsVisible = false;
        //        int count = mainViewModel.FrpConfigs.Where(p => p.ProcessStatus == ProcessStatus.Running).Count();
        //        if (await desktop.MainWindow.ShowYesNoDialogAsync("退出", $"存在{count}个正在运行的frp进程，是否退出？") == true)
        //        {
        //            foreach (var frp in mainViewModel.FrpConfigs)
        //            {
        //                await frp.StopAsync();
        //            }
        //            desktop.MainWindow.Close();
        //        }
        //    }
        //    else
        //    {
        //        desktop.MainWindow.Close();
        //    }
        //}
        //else
        //{
        //    throw new PlatformNotSupportedException();
        //}
    }

    private void HideMenuItem_Click(object sender, EventArgs e)
    {
        TrayIcon.GetIcons(this)[0].IsVisible = false;
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
    private void TrayIcon_Clicked(object sender, EventArgs e)
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
