using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;

namespace FrpGUI
{
    public class ProcessHelper
    {
        public bool IsRunning { get; set; }

        private Process frpProcess;
        private string type;
        private IToIni obj;

        public void StartServer(IToIni obj)
        {
            Start("s", obj);
        }

        public void StartClient(IToIni obj)
        {
            Start("c", obj);
        }

        public void Start(string type, IToIni obj)
        {
            (App.Current.MainWindow as MainWindow).AddLogOnMainThread("正在启动"+type, "I");

              if (frpProcess != null)
            {
                frpProcess.Kill();
            }
            this.type = type;
            this.obj = obj;
            string ini = Path.GetTempFileName() + ".ini";
            File.WriteAllText(ini, obj.ToIni());
            frpProcess = new Process();
            frpProcess.StartInfo = new ProcessStartInfo()
            {
                FileName = $"./frp/frp{type}.exe",
                Arguments = $"-c \"{ini}\"",
                WorkingDirectory = "./frp",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };
            frpProcess.EnableRaisingEvents = true;
            frpProcess.OutputDataReceived += P_OutputDataReceived;
            frpProcess.ErrorDataReceived += P_OutputDataReceived;
            frpProcess.Start();
            frpProcess.BeginOutputReadLine();
            frpProcess.BeginErrorReadLine();
            frpProcess.Exited += FrpProcess_Exited;
            IsRunning = true;
            Started?.Invoke(this, new EventArgs());
        }

        public async Task<Process[]> GetExistedProcesses(string type)
        {
            Process[] existProcess = null;
            await Task.Run(() =>
            {
                existProcess = Process.GetProcessesByName($"frp{type}");
            });
            return existProcess;
        }

        public async Task KillExistedProcesses(string type)
        {
            Process[] existProcess = null;
            await Task.Run(() =>
            {
                existProcess = Process.GetProcessesByName($"frp{type}");
            });
            if (existProcess.Length > 0)
            {
                foreach (var p in existProcess)
                {
                    p.Kill(true);
                }
            }
        }

        private void FrpProcess_Exited(object sender, EventArgs e)
        {
            IsRunning = false;
            frpProcess.Dispose();
            frpProcess = null;
            Exited?.Invoke(sender, e);
        }

        public async Task RestartAsync()
        {
            if (frpProcess == null)
            {
                throw new Exception();
            }
            await StopAsync();
            Start(type, obj);
        }

        public Task StopAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            IsRunning = false;
            frpProcess.Exited -= FrpProcess_Exited;
            frpProcess.Exited += (p1, p2) =>
            {
                frpProcess.Dispose();
                int code = 0;
                try
                {
                    code = frpProcess.ExitCode;
                }
                catch
                {
                }
                frpProcess = null;
                Exited?.Invoke(this, new EventArgs());
                tcs.SetResult(code);
            };
            frpProcess.Kill(true);
            return tcs.Task;
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }
            Debug.WriteLine(e.Data);
            Output?.Invoke(sender, e);
        }

        public static event DataReceivedEventHandler Output;

        public event EventHandler Exited;

        public event EventHandler Started;
    }
}