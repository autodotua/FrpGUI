using FrpGUI.Config;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrpGUI.Util
{
    public class ProcessHelper(FrpConfigBase frpConfig)
    {
        public bool IsRunning { get; set; }

        public FrpConfigBase FrpConfig { get; } = frpConfig;

        private Process frpProcess;

        private char type;

        private IToFrpConfig obj;

        public void Start(char type, IToFrpConfig obj)
        {
            if (type is not ('c' or 's'))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            Logger.Info($"正在启动", FrpConfig.Name);

            bool processHasStarted = false;
            try
            {
                try
                {
                    frpProcess?.Kill();
                }
                catch
                {

                }
                this.type = type;
                this.obj = obj;
                string tempDir = Path.Combine(FzLib.Program.App.ProgramDirectoryPath, "temp");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                string configFile = null;
                switch (AppConfig.Instance.FrpConfigType)
                {
                    case "INI":
                        throw new NotImplementedException();
                    case "TOML":
                        configFile = Path.Combine(tempDir, Guid.NewGuid().ToString() + ".toml");
                        File.WriteAllText(configFile, obj.ToToml(), new UTF8Encoding(false));
                        break;
                    default:
                        throw new Exception("未知FRP配置文件类型");
                }
                Logger.Info("配置文件地址：" + configFile, FrpConfig.Name);
                string frpExe = $"./frp/frp{type}";
                if (!File.Exists(frpExe) && !File.Exists(frpExe + ".exe"))
                {
                    throw new FileNotFoundException("没有找到frp程序，请将可执行文件放置在./frp/中");
                }
                frpProcess = new Process();
                frpProcess.StartInfo = new ProcessStartInfo()
                {
                    FileName = frpExe,
                    Arguments = $"-c \"{configFile}\"",
                    WorkingDirectory = "./frp",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardInputEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                };
                frpProcess.EnableRaisingEvents = true;
                frpProcess.OutputDataReceived += P_OutputDataReceived;
                frpProcess.ErrorDataReceived += P_OutputDataReceived;
                processHasStarted = true;
                frpProcess.Start();
                frpProcess.BeginOutputReadLine();
                frpProcess.BeginErrorReadLine();
                frpProcess.Exited += FrpProcess_Exited;
                IsRunning = true;
                Started?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Logger.Error("启动失败：" + ex.Message, FrpConfig.Name, ex);
                if (processHasStarted)
                {
                    try
                    {
                        frpProcess.Kill();
                        frpProcess.Dispose();
                    }
                    catch
                    {

                    }
                }
                throw;
            }
        }

        public async Task<Process[]> GetExistedProcesses(char type)
        {
            Process[] existProcess = null;
            await Task.Run(() =>
            {
                existProcess = Process.GetProcessesByName($"frp{type}");
            });
            return existProcess;
        }

        public async Task KillExistedProcesses(char type)
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
            if (frpProcess == null)
            {
                return Task.CompletedTask;
            }
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
            Logger.Output(e.Data, FrpConfig.Name);
        }

        public event EventHandler Exited;

        public event EventHandler Started;
    }
}