using FrpGUI.Configs;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

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