using System.Threading;
using FrpGUI.Configs;
using FzLib.Services;
using FrpGUI.Models;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia;

public partial class App
{
    public class LocalAppLifetimeService(AppConfig config,UIConfig uiconfig, LoggerBase logger, FrpProcessCollection processes) 
        : AppLifetimeService(config,logger,processes)
    {
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            uiconfig.Save();
            return base.StopAsync(cancellationToken);
        }
    }
}
