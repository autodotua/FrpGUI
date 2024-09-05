using FrpGUI.Configs;
using FrpGUI.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

public abstract class FrpControllerBase : ControllerBase
{
    private AppConfig config;
    private Logger logger;

    public FrpControllerBase(AppConfig config, Logger logger)
    {
        this.config = config;
        this.logger = logger;
    }
}

