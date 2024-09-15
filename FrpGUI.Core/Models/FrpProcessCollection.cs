using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using System.Diagnostics;

namespace FrpGUI.Models;

public class FrpProcessCollection(AppConfig config, LoggerBase logger) : Dictionary<string, FrpProcess>
{
    public IFrpProcess GetOrCreateProcess(string id)
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

    public async Task<FrpConfigBase> RemoveFrpAsync(string id)
    {
        var frp = GetOrCreateProcess(id);
        if (frp.ProcessStatus == ProcessStatus.Running)
        {
            await frp.StopAsync();
        }
        config.FrpConfigs.Remove(frp.Config);
        Remove(frp.Config.ID);
        config.Save();
        return frp.Config;
    }
}
