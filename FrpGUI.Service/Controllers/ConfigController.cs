using FrpGUI.Configs;
using FrpGUI.Service.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigController : FrpControllerBase
{
    private readonly FrpProcessService processes;

    public ConfigController(AppConfig configs, Logger logger, FrpProcessService processes) : base(configs, logger)
    {
        this.processes = processes;
    }

    [HttpGet("FrpConfigs")]
    public IList<FrpConfigBase> GetFrpConfigList()
    {
        return configs.FrpConfigs;
    }

    [HttpPost("FrpConfigs/Delete/{id}")]
    public async Task DeleteFrpConfigAsync(string id)
    {
        var frp = processes.GetOrCreateProcess(id);
        logger.Info($"指令：删除配置", frp.Config);
        if (frp.ProcessStatus == Enums.ProcessStatus.Running)
        {
            await frp.StopAsync();
        }
        configs.FrpConfigs.Remove(frp.Config);
        processes.Remove(frp.Config.ID);
        configs.Save();
    }

    [HttpPost("FrpConfigs/Add/Client")]
    public ClientConfig AddClientConfig()
    {
        logger.Info($"指令：新增客户端");
        ClientConfig clientConfig = new ClientConfig();
        configs.FrpConfigs.Add(clientConfig);
        configs.Save();
        return clientConfig;
    }

    [HttpPost("FrpConfigs/Add/Server")]
    public ServerConfig AddServerConfig()
    {
        logger.Info($"指令：新增服务端");
        ServerConfig serverConfig = new ServerConfig();
        configs.FrpConfigs.Add(serverConfig);
        configs.Save();
        return serverConfig;
    }

    [HttpPost("FrpConfigs/Modify")]
    public void ModifyConfig(FrpConfigBase config)
    {
        var p = processes.GetOrCreateProcess(config.ID);
        logger.Info($"指令：应用配置", p.Config);
        if (p.Config.GetType() != config.GetType())
        {
            throw new ArgumentException("提供的配置与已有配置类型不同");
        }
        configs.Adapt(p.Config);
        configs.Save();
    }
}