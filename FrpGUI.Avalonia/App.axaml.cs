using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaWebView;
using FrpGUI.Avalonia.DataProviders;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using FrpGUI.Services;
using FzLib.Avalonia.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using FzLib.Program.Startup;

namespace FrpGUI.Avalonia;

public partial class App : Application
{
    public App()
    {
    }

    public static IServiceProvider Services { get; private set; }

    public IHost AppHost { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        //Windows端加载内置浏览器
        if (OperatingSystem.IsWindows())
        {
            //由于浏览器总在最上层，因此需要使用Window形式的对话框
            DialogExtension.ContainerType = DialogContainerType.WindowPreferred;
            AvaloniaWebViewBuilder.Initialize(default);
        }

        //Windows上使用微软雅黑
        if (OperatingSystem.IsWindows())
        {
            Resources.Add("ContentControlThemeFontFamily", new FontFamily("Microsoft YaHei"));
        }

        //浏览器端需要设置内置字体才可正常显示中文
        else if (OperatingSystem.IsBrowser())
        {
            Resources.Add("ContentControlThemeFontFamily",
                new FontFamily("avares://FrpGUI.Avalonia/Assets#Microsoft YaHei"));
        }

        var builder = Host.CreateApplicationBuilder();
        var uiconfig = AppConfigBase.Get<UIConfig>();

        //浏览器一定是使用服务模式而不是单机模式
        if (OperatingSystem.IsBrowser())
        {
            if (uiconfig.RunningMode == RunningMode.Singleton)
            {
                uiconfig.RunningMode = RunningMode.Service;
            }
        }

        switch (uiconfig.RunningMode)
        {
            case RunningMode.Singleton:
                builder.Services.AddSingleton<IDataProvider, LocalDataProvider>();
                builder.Services.AddHostedService<LocalAppLifetimeService>();
                builder.Services.AddSingleton<FrpProcessCollection>();
                builder.Services.AddSingleton(AppConfigBase.Get<AppConfig>());
                break;

            case RunningMode.Service:
                builder.Services.AddSingleton<IDataProvider, WebDataProvider>();
                break;
        }

        var logger = new LocalLogger();
        builder.Services.AddSingleton<LoggerBase>(logger);
        builder.Services.AddSingleton<LocalLogger>(logger);
        builder.Services.AddStartupManager();

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

        builder.Services.AddSingleton(uiconfig);

        AppHost = builder.Build();

        Services = AppHost.Services;
        AppHost.Start();
    }

    private MainWindow mainWindow;

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);
            mainWindow = Services.GetRequiredService<MainWindow>();
            var startup = desktop.Args is { Length: > 0 } && desktop.Args[0] == "s";
            if (!startup)
            {
                desktop.MainWindow = mainWindow;
            }

            desktop.Exit += Desktop_Exit;

            InitializeTrayIcon(startup);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime s)
        {
            s.MainView = new MainView();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public async Task ShutdownAsync()
    {
        await AppHost.StopAsync();
        Environment.Exit(0);
    }

    private async void Desktop_Exit(object sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        TrayIcon.GetIcons(this)[0].Dispose();
        await AppHost.StopAsync();
    }

    private async void ExitMenuItem_Click(object sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new PlatformNotSupportedException();
        }

        var mainWindow = desktop.MainWindow as MainWindow;
        await mainWindow.TryCloseAsync();
    }

    private void InitializeTrayIcon(bool force)
    {
        Services.GetRequiredService<UIConfig>().PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(UIConfig.ShowTrayIcon))
            {
                TrayIcon.GetIcons(this)[0].IsVisible = Services.GetRequiredService<UIConfig>().ShowTrayIcon;
            }
        };
        TrayIcon.GetIcons(this)[0].IsVisible = force || Services.GetRequiredService<UIConfig>().ShowTrayIcon;
    }

    private void TrayIcon_Clicked(object sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow == null)
            {
                desktop.MainWindow = mainWindow;
            }

            if (desktop.MainWindow.IsVisible)
            {
                desktop.MainWindow.Hide();
            }
            else
            {
                desktop.MainWindow.Show();
            }
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }
}