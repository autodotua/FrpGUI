using FrpGUI.Configs;
using FrpGUI.Service.Models;

namespace FrpGUI.Service
{
    public class FrpProcessManager(AppConfig config, Logger logger) : Dictionary<FrpConfigBase, FrpProcess>, IHostedService
    {
   
        public FrpProcess GetOrCreateProcess(string id)
        {
            var frp = GetFrpConfig(id);
            if (TryGetValue(frp, out FrpProcess process))
            {
                return process;
            }
            process = new FrpProcess(frp, logger);
            return process;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var frpConfig in Keys)
            {
                if (frpConfig.AutoStart)
                {
                    await GetOrCreateProcess(frpConfig.ID).StartAsync();
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            config.Save();
            foreach (FrpProcess process in Values)
            {
                if (process.ProcessStatus == Enums.ProcessStatus.Running)
                {
                    await process.StopAsync();
                }
            }
        }

        protected FrpConfigBase GetFrpConfig(string id)
        {
            return config.FrpConfigs.FirstOrDefault(p => p.ID == id) ?? throw new ArgumentException($"找不到ID为{id}的配置");
        }
    }
}
