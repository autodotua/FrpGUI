using FrpGUI.Util;
using FzLib.Program;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace FrpGUI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TrayIcon tray;

        public void ShowTray()
        {
            tray.Show();
        }

        public void SetStartup(bool run)
        {
            if (run)
            {
                FzLib.Program.Startup.CreateRegistryKey("s");
            }
            else
            {
                FzLib.Program.Startup.DeleteRegistryKey();
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            UnhandledException.RegistAll();

            UnhandledException.UnhandledExceptionCatched += UnhandledException_UnhandledExceptionCatched;
            FzLib.Program.App.SetWorkingDirectoryToAppPath();
            FzLib.Program.Startup.AppName = Name;
            tray = new TrayIcon(new System.Drawing.Icon("./icon.ico"), App.Name);
            tray.ReShowWhenDisplayChanged = true;
            if (e.Args.Length > 0 && e.Args[0] == "s")
            {
                MainWindow = new MainWindow(true);
                ShowTray();
            }
            else
            {
                MainWindow = new MainWindow();
                MainWindow.Show();
            }

            tray.MouseLeftClick += (p1, p2) =>
            {
                tray.Hide();
                MainWindow.Visibility = Visibility.Visible;
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
                MainWindow.Focus();
            };

            await new HttpServerHelper().StartAsync();
        }

        private void UnhandledException_UnhandledExceptionCatched(object sender, UnhandledException.UnhandledExceptionEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    System.Windows.MessageBox.Show("发生异常：" + Environment.NewLine + e.Exception.ToString());
                    File.AppendAllText("exceptions.log", Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + e.Exception.ToString() + Environment.NewLine);
                }
                finally
                {
                    Shutdown();
                }
            });
        }

        public static string Name => "FrpGUI";
    }

    public static class UnhandledException
    {
        public delegate void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e);

        public static event UnhandledExceptionEventHandler UnhandledExceptionCatched;

        public static bool ExitAfterHandle { get; set; } = false;

        public static void RegistAll()
        {
            Regist();
        }

        private static void Regist()
        {
            TaskScheduler.UnobservedTaskException += (p1, p2) => //Task
            {
                if (!p2.Observed)
                {
                    RaiseEvent(p1, p2.Exception.InnerException, ExceptionSource.TaskScheduler);
                    try
                    {
                        p2.SetObserved();
                    }
                    catch
                    {
                    }
                }
            };
            AppDomain.CurrentDomain.UnhandledException += (p1, p2) =>//Thread
            {
                RaiseEvent(p1, (Exception)p2.ExceptionObject, ExceptionSource.AppDomain);
            };
            Application.Current.DispatcherUnhandledException += (p1, p2) =>//UI
            {
                RaiseEvent(p1, p2.Exception, ExceptionSource.Application);
                p2.Handled = true;
            };
        }

        private static void RaiseEvent(object sender, Exception ex, ExceptionSource source)
        {
            var e = new UnhandledExceptionEventArgs(ex, source);
            UnhandledExceptionCatched?.Invoke(sender, e);
        }

        public class UnhandledExceptionEventArgs : EventArgs
        {
            public UnhandledExceptionEventArgs(Exception exception, ExceptionSource source)
            {
                Exception = exception ?? throw new ArgumentNullException(nameof(exception));
                Source = source;
            }

            public Exception Exception { get; private set; }
            public ExceptionSource Source { get; private set; }
        }

        public enum ExceptionSource
        {
            TaskScheduler,
            AppDomain,
            Application,
        }
    }

    public class TrayIcon : IDisposable
    {
        private NotifyIcon trayIcon = new NotifyIcon();

        private TrayIcon()
        {
            trayIcon.MouseClick += (p1, p2) =>
            {
                if (p2.Button == MouseButtons.Left)
                {
                    MouseLeftClick?.Invoke(p1, p2);
                }
                else if (p2.Button == MouseButtons.Right)
                {
                    MouseRightClick?.Invoke(p1, p2);
                }
            };
        }

        public TrayIcon(System.Drawing.Icon icon, string text) : this()
        {
            trayIcon.Text = text;
            trayIcon.Icon = icon;
        }

        public TrayIcon(System.Drawing.Icon icon, string text, MouseEventHandler mouseClick) : this(icon, text)
        {
            if (mouseClick != null)
            {
                trayIcon.MouseClick += mouseClick;
            }
        }

        public TrayIcon(System.Drawing.Icon icon, string text, Action mouseLeftClick, Action mouseRightClick) : this(icon, text)
        {
            if (mouseLeftClick != null || mouseRightClick != null)
            {
                trayIcon.MouseClick += (p1, p2) =>
                {
                    if (p2.Button == MouseButtons.Left)
                    {
                        mouseLeftClick?.Invoke();
                    }
                    else if (p2.Button == MouseButtons.Right)
                    {
                        mouseRightClick?.Invoke();
                    }
                };
            }
        }

        public bool ReShowWhenDisplayChanged
        {
            get => reShowWhenDpiChanged;
            set
            {
                if (value == reShowWhenDpiChanged)
                {
                    return;
                }

                reShowWhenDpiChanged = value;
                if (value)
                {
                    Microsoft.Win32.SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
                }
                else
                {
                    Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= DisplaySettingsChanged;
                }
            }
        }

        private void DisplaySettingsChanged(object sender, EventArgs e)
        {
            Hide();
            Show();
        }

        private bool reShowWhenDpiChanged = false;

        public TrayIcon(System.Drawing.Icon icon, string text, Action mouseLeftClick, Dictionary<string, Action> mouseRightClickMenu) : this(icon, text)
        {
            if (mouseLeftClick != null)
            {
                trayIcon.MouseClick += (p1, p2) =>
                {
                    if (p2.Button == MouseButtons.Left)
                    {
                        mouseLeftClick?.Invoke();
                    }
                };
            }

            if (mouseRightClickMenu != null && mouseRightClickMenu.Count != 0)
            {
                AddContextMenuStripItems(mouseRightClickMenu);
            }
        }

        public void AddContextMenuStripItems(Dictionary<string, Action> mouseRightClickMenu)
        {
            if (mouseRightClickMenu == null)
            {
                throw new ArgumentNullException();
            }
            if (mouseRightClickMenu.Count == 0)
            {
                return;
            }
            if (trayIcon.ContextMenuStrip == null)
            {
                trayIcon.ContextMenuStrip = new ContextMenuStrip();
            }
            foreach (var item in mouseRightClickMenu)
            {
                trayIcon.ContextMenuStrip.Items.Add(item.Key, null, new EventHandler((p1, p2) => item.Value()));
            }
        }

        public void AddContextMenuStripItem(string text, Action action)
        {
            if (text == null || action == null)
            {
                throw new ArgumentNullException();
            }
            if (trayIcon.ContextMenuStrip == null)
            {
                trayIcon.ContextMenuStrip = new ContextMenuStrip();
            }
            trayIcon.ContextMenuStrip.Items.Add(text, null, new EventHandler((p1, p2) => action()));
        }

        public void InsertContextMenuStripItem(string text, Action action, int index)
        {
            if (text == null || action == null)
            {
                throw new ArgumentNullException();
            }
            if (trayIcon.ContextMenuStrip == null)
            {
                trayIcon.ContextMenuStrip = new ContextMenuStrip();
            }
            var item = new ToolStripMenuItem(text, null, new EventHandler((p1, p2) => action()));
            trayIcon.ContextMenuStrip.Items.Insert(index, item);
        }

        public void DeleteContextMenuStripItem(string text)
        {
            trayIcon.ContextMenuStrip.Items.RemoveByKey(text);
        }

        public void ClearContextMenuStripItems()
        {
            if (trayIcon.ContextMenuStrip != null)
            {
                trayIcon.ContextMenuStrip.Items.Clear();
            }
        }

        public ContextMenuStrip ContextMenuStrip => trayIcon.ContextMenuStrip;

        public void ClickToOpenOrHideWindow(Window window)
        {
            MouseLeftClick += (p1, p2) =>
            {
                if (window.Visibility != Visibility.Visible)
                {
                    window.Show();
                    window.Activate();
                }
                else
                {
                    window.Hide();
                }
            };
        }

        public event MouseEventHandler MouseLeftClick;

        public event MouseEventHandler MouseRightClick;

        public void ShowMessage(string message, int ms = 2000)
        {
            trayIcon.BalloonTipText = message;
            trayIcon.ShowBalloonTip(ms);
        }

        public void Show()
        {
            trayIcon.Visible = true;
        }

        public void Hide()
        {
            trayIcon.Visible = false;
        }

        public void Dispose()
        {
            trayIcon.Dispose();
        }
    }
}