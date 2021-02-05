using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FrpGUI
{
    public class ProcessHelper
    {
        public bool IsRunning { get; set; }

        private Process frpProcess;
        private string type;
        private IToIni obj;

        public void Start(string type, IToIni obj)
        {
            this.type = type;
            this.obj = obj;
            var existProcess = Process.GetProcessesByName($"frp{type}");

            if (existProcess.Length > 0)
            {
                foreach (var p in existProcess)
                {
                    p.Kill(true);
                }
            }
            string ini = $"./frp/frp{type}.ini";
            File.WriteAllText(ini, obj.ToIni());
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
            frpProcess.Exited += (p1, p2) => Exited?.Invoke(p1, p2);
        }

        public void Restart()
        {
            if (frpProcess == null)
            {
                throw new Exception();
            }
            Stop();
            Start(type, obj);
        }

        public void Stop()
        {
            frpProcess.Kill(true);
            frpProcess = null;
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