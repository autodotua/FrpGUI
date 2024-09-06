using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Service.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;

namespace FrpGUI.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ProcessController : FrpControllerBase
{
    private readonly AppConfig config;
    private readonly Logger logger;
    private readonly FrpProcessService processes;

    public ProcessController(AppConfig config, Logger logger, FrpProcessService processes) : base(config, logger)
    {
        this.config = config;
        this.logger = logger;
        this.processes = processes;
    }



    [HttpPost("Start/{id}")]
    public Task StartAsync(string id)
    {
        return processes.GetOrCreateProcess(id).StartAsync();
    }

    [HttpPost("Stop/{id}")]
    public Task StopAsync(string id)
    {
        return processes.GetOrCreateProcess(id).StopAsync();
    }

    [HttpPost("Restart/{id}")]
    public Task RestartAsync(string id)
    {
        return processes.GetOrCreateProcess(id).RestartAsync();
    }

    [HttpGet("Status/{id}")]
    public ProcessStatus GetStatusAsync(string id)
    {
        return processes.GetOrCreateProcess(id).ProcessStatus;
    }
}

