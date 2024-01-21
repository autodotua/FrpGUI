using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using log4net;
using log4net.Appender;
using log4net.Layout;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace FrpGUI.Avalonia.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        InitializeLogs();
        Logger.NewLog += Logger_NewLog;
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Fatal("未捕获的异常", ex);
        }
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Fatal("未捕获的AppDomain异常", e.ExceptionObject as Exception);
    }

    private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Fatal("未捕获的TaskScheduler异常", e.Exception);
    }

    private static void Logger_NewLog(object sender, LogEventArgs e)
    {
        switch (e.Type)
        {
            case 'I':
                Log.Info(e.Message);
                break;
            case 'E':
                Log.Error(e.Message);
                break;
            case 'W':
                Log.Warn(e.Message);
                break;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    public static ILog Log { get; private set; }
    /// <summary>
    /// 初始化日志系统
    /// </summary>
    private static void InitializeLogs()
    {
        Log = LogManager.GetLogger(typeof(Program));
        RollingFileAppender fa = new RollingFileAppender()
        {
            AppendToFile = true,
            MaxSizeRollBackups = 100,
            StaticLogFileName = true,
            RollingStyle = RollingFileAppender.RollingMode.Date,
            Layout = new PatternLayout("[%date]-[%thread]-[%-p]%newline%message%newline%newline"),
            File = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "logs", "FrpGUI.log"),
        };
        fa.ActivateOptions();
        ((log4net.Repository.Hierarchy.Logger)Log.Logger).AddAppender(fa);

        Log.Info("程序启动");
    }
}
