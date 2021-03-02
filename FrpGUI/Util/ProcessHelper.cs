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

        public async Task StartAsync(string type, IToIni obj)
        {
            if (frpProcess != null)
            {
                //throw new Exception("存在仍在运行的Frp实例");
            }
            this.type = type;
            this.obj = obj;
            Process[] existProcess = null;
            await Task.Run(() =>
            {
                string ini = $"./frp/frp{type}.ini";
                File.WriteAllText(ini, obj.ToIni());
                existProcess = Process.GetProcessesByName($"frp{type}");
            });
            if (existProcess.Length > 0)
            {
                foreach (var p in existProcess)
                {
                    p.Kill(true);
                }
                await Task.Delay(500);
            }
            frpProcess = new Process();
            frpProcess.StartInfo = new ProcessStartInfo()
            {
                FileName = $"./frp/frp{type}.exe",
                Arguments = $"-c frp{type}.ini",
                WorkingDirectory = "./frp",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            frpProcess.EnableRaisingEvents = true;
            frpProcess.OutputDataReceived += P_OutputDataReceived;
            frpProcess.ErrorDataReceived += P_OutputDataReceived;
            frpProcess.Start();
            frpProcess.BeginOutputReadLine();
            frpProcess.BeginErrorReadLine();
            frpProcess.Exited += FrpProcess_Exited;
        }

        private void FrpProcess_Exited(object sender, EventArgs e)
        {
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
            await StartAsync(type, obj);
        }

        public Task StopAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            frpProcess.Exited -= FrpProcess_Exited;
            frpProcess.Exited += (p1, p2) =>
            {
                tcs.SetResult(frpProcess.ExitCode);
                frpProcess.Dispose();
                frpProcess = null;
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
    }
}