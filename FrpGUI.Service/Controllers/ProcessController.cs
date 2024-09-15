using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using FrpGUI.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

[NeedToken]
[ApiController]
[Route("[controller]")]
public class ProcessController : FrpControllerBase
{
    private readonly FrpProcessCollection processes;

    public ProcessController(AppConfig config, LoggerBase logger, FrpProcessCollection processes) : base(config, logger)
    {
        this.processes = processes;
    }

    [HttpGet("Status")]
    public IList<IFrpProcess> GetFrpProcessList()
    {
        return processes.GetAll();
    }

    [HttpPost("Start/{id}")]
    public Task StartAsync(string id)
    {
        var frp = processes.GetOrCreateProcess(id);
        logger.Info($"指令：启动", frp.Config);
        return frp.StartAsync();
    }

    [HttpPost("Stop/{id}")]
    public Task StopAsync(string id)
    {
        var frp = processes.GetOrCreateProcess(id);
        logger.Info($"指令：停止", frp.Config);
        return frp.StopAsync();
    }

    [HttpPost("Restart/{id}")]
    public Task RestartAsync(string id)
    {
        var frp = processes.GetOrCreateProcess(id);
        logger.Info($"指令：重启", frp.Config);
        return frp.RestartAsync();
    }

    [HttpGet("Status/{id}")]
    public ProcessStatus GetStatusAsync(string id)
    {
        return processes.GetOrCreateProcess(id).ProcessStatus;
    }
}