using FrpGUI.Configs;
using FrpGUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.WebAPI.Controllers;

public abstract class FrpControllerBase : ControllerBase
{
    protected AppConfig configs;
    protected LoggerBase logger;

    public FrpControllerBase(AppConfig configs, LoggerBase logger)
    {
        this.configs = configs;
        this.logger = logger;
    }
}