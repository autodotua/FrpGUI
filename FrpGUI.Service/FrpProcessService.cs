using FrpGUI.Configs;
using FrpGUI.Models;
using FrpGUI.Service.Models;

namespace FrpGUI.Service
{
    public class FrpProcessService(AppConfig config, Logger logger) : Dictionary<string, FrpProcess>
    {
        public FrpProcess GetOrCreateProcess(string id)
        {
            if (TryGetValue(id, out FrpProcess process))
            {
                return process;
            }
            var frp = GetFrpConfig(id);
            process = new FrpProcess(frp, logger);
            Add(id, process);
            return process;
        }

        protected FrpConfigBase GetFrpConfig(string id)
        {
            return config.FrpConfigs.FirstOrDefault(p => p.ID == id) ?? throw new ArgumentException($"找不到ID为{id}的配置");
        }

        public IList<IFrpProcess> GetAll()
        {
            List<IFrpProcess> list = new List<IFrpProcess>();
            foreach (var item in config.FrpConfigs)
            {
                list.Add(GetOrCreateProcess(item.ID));
            }
            return list;
        }
    }
}
