using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using FrpGUI.Service.Util;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace FrpGUI.Service.Models
{
    public class FrpProcess : IFrpProcess
    {
        private readonly Logger logger;

        public FrpProcess(FrpConfigBase config, Logger logger)
        {
            Config = config;
            this.logger = logger;
            Process = new ProcessHelper(Config, logger);
            Process.Exited += Process_Exited;
        }

        public ProcessHelper Process { get; protected set; }

        public FrpConfigBase Config { get; }

        public ProcessStatus ProcessStatus { get; set; }

        public async Task RestartAsync()
        {
            if (ProcessStatus == ProcessStatus.Stopped)
            {
                throw new Exception("进程未在运行");
            }
            ChangeStatus(ProcessStatus.Busy);
            try
            {
                await Process.RestartAsync();
            }
            catch (Exception ex)
            {
                ChangeStatus(ProcessStatus.Stopped);
                throw;
            }
            ChangeStatus(ProcessStatus.Running);
        }

        public Task StartAsync()
        {
            if (ProcessStatus == ProcessStatus.Running)
            {
                throw new Exception("进程已在运行");
            }
            ChangeStatus(ProcessStatus.Busy);
            try
            {
                Process.Start();
                ChangeStatus(ProcessStatus.Running);
            }
            catch (Exception ex)
            {
                ChangeStatus(ProcessStatus.Stopped);
                throw;
            }
            return Task.CompletedTask;
        }

        public void ChangeStatus(ProcessStatus status)
        {
            logger.Info("进程状态改变：" + status.ToString(), Config);
            ProcessStatus = status;
            StatusChanged?.Invoke(this, new EventArgs());
        }

        public async Task StopAsync()
        {
            if (ProcessStatus == ProcessStatus.Stopped)
            {
                throw new Exception("进程未在运行");
            }
            ChangeStatus(ProcessStatus.Busy);
            await Process.StopAsync();
            ChangeStatus(ProcessStatus.Stopped);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            ChangeStatus(ProcessStatus.Stopped);
        }


        public event EventHandler StatusChanged;
    }
}
