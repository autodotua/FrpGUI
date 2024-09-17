using FrpGUI.Configs;
using FrpGUI.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

[NeedToken]
[ApiController]
[Route("[controller]")]
public class ConfigController : FrpControllerBase
{
    private readonly FrpProcessCollection processes;

    public ConfigController(AppConfig configs, LoggerBase logger, FrpProcessCollection processes) : base(configs, logger)
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
        var frp=await processes.RemoveFrpAsync(id);
        logger.Info($"指令：删除配置", frp);
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
        //需要指定实际的类型，不然只会Adapt基类属性
        if(config is ClientConfig c)
        {
            c.Adapt(p.Config as ClientConfig);
        }
        else if(config is ServerConfig s)
        {
            s.Adapt(p.Config as ServerConfig);
        }
        configs.Save();
    }
}