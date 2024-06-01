using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using System;
using System.IO.Pipes;
using System.IO;
using System.Threading;
using Avalonia.Controls;

namespace FrpGUI.Avalonia
{
    public class SingleRunningAppHelper(string name) : IDisposable
    {

        private static Mutex mutex;

        public string Name { get; } = name;

        public void Dispose()
        {
            mutex?.Dispose();
        }

        public bool Register()
        {
            mutex = new Mutex(true, Name, out bool createdNew);

            if (!createdNew)
            {
                NotifyExistingInstance();
                return true;
            }
            return false;
        }

        public void NotifyExistingInstance()
        {
            using var client = new NamedPipeClientStream(Name);
            try
            {
                client.Connect(1000);
                using var writer = new StreamWriter(client);
                writer.WriteLine("Activate");
                writer.Flush();
            }
            catch (TimeoutException)
            {
                // Handle exception (if any)
            }
        }

        public void StartListening()
        {
            var thread = new Thread(ListenForActivationMessages);
            thread.IsBackground = true;
            thread.Start();
        }

        private void ListenForActivationMessages()
        {
            while (true)
            {
                using var pipeServer = new NamedPipeServerStream(Name);
                pipeServer.WaitForConnection();
                using var reader = new StreamReader(pipeServer);
                var message = reader.ReadLine();
                if (message == "Activate")
                {
                    var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (mainWindow.IsVisible == false)
                        {
                            mainWindow.Show();
                        }
                        if (mainWindow.WindowState == WindowState.Minimized)
                        {
                            mainWindow.WindowState = WindowState.Normal;
                        }
                        mainWindow.Activate();
                    });
                }
            }

        }

    }
}