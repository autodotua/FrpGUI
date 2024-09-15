using FrpGUI;
using FrpGUI.Configs;
using FrpGUI.Models;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

namespace FzLib.Services
{
    public class AppLifetimeService(AppConfig config, LoggerBase logger, FrpProcessCollection processes) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var fp in processes.GetAll())
            {
                if (fp.Config.AutoStart)
                {
                    logger.Info($"自动启动：{fp.Config.Name}");
                    try
                    {
                        await processes.GetOrCreateProcess(fp.Config.ID).StartAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"自动启动{fp.Config.Name}失败", fp.Config, ex);
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            config.Save();
            foreach (FrpProcess fp in processes.Values)
            {
                if (fp.ProcessStatus == FrpGUI.Enums.ProcessStatus.Running)
                {
                    logger.Info($"应用正在退出，正在停止：{fp.Config.Name}");
                    try
                    {
                        await fp.StopAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"停止{fp.Config.Name}失败", fp.Config, ex);
                    }
                }
            }
        }
    }
}
