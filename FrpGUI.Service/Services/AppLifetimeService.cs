using FrpGUI;
using FrpGUI.Configs;
using FrpGUI.Models;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

namespace FrpGUI.Services
{
    public class AppLifetimeService(AppConfig config, LoggerBase logger, FrpProcessCollection processes) : IHostedService
    {
        protected AppConfig Config { get; } = config;
        protected LoggerBase Logger { get; } = logger;
        protected FrpProcessCollection Processes { get; } = processes;

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var fp in Processes.GetAll())
            {
                if (fp.Config.AutoStart)
                {
                    Logger.Info($"自动启动：{fp.Config.Name}");
                    try
                    {
                        await Processes.GetOrCreateProcess(fp.Config.ID).StartAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"自动启动{fp.Config.Name}失败", fp.Config, ex);
                    }
                }
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            Config.Save();
            foreach (FrpProcess fp in Processes.Values)
            {
                if (fp.ProcessStatus == FrpGUI.Enums.ProcessStatus.Running)
                {
                    Logger.Info($"应用正在退出，正在停止：{fp.Config.Name}");
                    try
                    {
                        await fp.StopAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"停止{fp.Config.Name}失败", fp.Config, ex);
                    }
                }
            }
        }
    }
}