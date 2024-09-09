using FrpGUI.Configs;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

public abstract class FrpControllerBase : ControllerBase
{
    protected AppConfig configs;
    protected Logger logger;

    public FrpControllerBase(AppConfig configs, Logger logger)
    {
        this.configs = configs;
        this.logger = logger;
    }
}