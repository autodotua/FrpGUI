using FrpGUI.Configs;
using FrpGUI.Service.Models;

namespace FrpGUI.Service
{
    public class FrpProcessService(AppConfig config, Logger logger) : Dictionary<FrpConfigBase, FrpProcess>
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

        protected FrpConfigBase GetFrpConfig(string id)
        {
            return config.FrpConfigs.FirstOrDefault(p => p.ID == id) ?? throw new ArgumentException($"找不到ID为{id}的配置");
        }
    }
}
