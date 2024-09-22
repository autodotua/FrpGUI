using FrpGUI.Configs;
using FrpGUI.Models;
using FrpGUI.Services;
using FrpGUI.WebAPI.Services;

namespace FrpGUI.WebAPI;

public class WebAppLifetimeService(AppConfig config, LoggerBase logger, FrpProcessCollection processes, WebConfigService serverConfigService) :
    AppLifetimeService(config, logger, processes)
{
    private readonly WebConfigService serverConfigService = serverConfigService;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        if (serverConfigService.ServerOnly())
        {
            foreach (var c in Config.FrpConfigs.ToList())
            {
                if (c is ClientConfig)
                {
                    Config.FrpConfigs.Remove(c);
                }
            }
        }
        return base.StartAsync(cancellationToken);
    }
}
