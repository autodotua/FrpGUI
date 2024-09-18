using Avalonia;
using log4net;
using System;
using System.IO;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace FrpGUI.Avalonia.Desktop;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Directory.SetCurrentDirectory(FzLib.Program.App.ProgramDirectoryPath);
        InitializeLogs();
#if !DEBUG
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        try
        {
#endif
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
#if !DEBUG
        }
        catch (Exception ex)
        {
            Log.Fatal("未捕获的异常", ex);
        }
        finally
        {
            //singleRunningApp.Dispose();
        }
#endif
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Fatal("未捕获的AppDomain异常", e.ExceptionObject as Exception);
    }

    private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Fatal("未捕获的TaskScheduler异常", e.Exception);
    }

    //private static void Logger_NewLog(object sender, LogEventArgs e)
    //{
    //    switch (e.Type)
    //    {
    //        case 'I':
    //            Log.Info(e.Message);
    //            break;
    //        case 'E':
    //            if (e.Exception == null)
    //            {
    //                Log.Error(e.Message);
    //            }
    //            else
    //            {
    //                Log.Error(e.Message, e.Exception);
    //            }
    //            break;
    //        case 'W':
    //            Log.Warn(e.Message);
    //            break;
    //    }
    //}

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

        Log.Info("程序启动");
    }
}