using FrpGUI.Configs;
using FrpGUI.Service.Models;

namespace FrpGUI.Service.Services
{
    public class AppLifetimeService(AppConfig config, Logger logger, FrpProcessService processes) : IHostedService
    {

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var frpConfig in processes.Values)
            {
                if (frpConfig.Config.AutoStart)
                {
                    await processes.GetOrCreateProcess(frpConfig.Config.ID).StartAsync();
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            config.Save();
            foreach (FrpProcess process in processes.Values)
            {
                if (process.ProcessStatus == Enums.ProcessStatus.Running)
                {
                    await process.StopAsync();
                }
            }
        }
    }
}
