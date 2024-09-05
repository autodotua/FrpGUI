using FrpGUI.Configs;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigController : FrpControllerBase
{
    private readonly AppConfig configs;
    private readonly FrpProcessManager processes;

    public ConfigController(AppConfig configs, Logger logger, FrpProcessManager processes) : base(configs, logger)
    {
        this.configs = configs;
        this.processes = processes;
    }

    [HttpGet("FrpConfigs")]
    public List<FrpConfigBase> GetFrpConfigList()
    {
        return configs.FrpConfigs;
    }

    [HttpPost("FrpConfigs/Delete")]
    public async Task DeleteFrpConfigAsync(string id)
    {
        var frp = processes.GetOrCreateProcess(id);
        if (frp.ProcessStatus == Enums.ProcessStatus.Running)
        {
            await frp.StopAsync();
        }
        configs.FrpConfigs.Remove(frp.Config);
        processes.Remove(frp.Config);
    }

    [HttpPost("FrpConfigs/Add/Client")]
    public ClientConfig AddClientConfig()
    {
        ClientConfig clientConfig = new ClientConfig();
        configs.FrpConfigs.Add(clientConfig);
        configs.Save();
        return clientConfig;
    }

    [HttpPost("FrpConfigs/Add/Server")]
    public ServerConfig AddServerConfig()
    {
        ServerConfig serverConfig = new ServerConfig();
        configs.FrpConfigs.Add(serverConfig);
        configs.Save();
        return serverConfig;
    }

    [HttpPost("FrpConfigs/Modify")]
    public void ModifyConfig(FrpConfigBase config)
    {
        var p = processes.GetOrCreateProcess(config.ID);
        if (p.Config.GetType() != config.GetType())
        {
            throw new ArgumentException("提供的配置与已有配置类型不同");
        }
        configs.Adapt(p.Config);
    }
}