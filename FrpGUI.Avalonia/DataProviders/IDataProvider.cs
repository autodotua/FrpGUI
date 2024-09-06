using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrpGUI.Avalonia.Models;
using FrpGUI.Configs;
using FrpGUI.Models;

namespace FrpGUI.Avalonia.DataProviders
{
    public interface IDataProvider
    {
        Task<ClientConfig> AddClientAsync();
        Task<ServerConfig> AddServerAsync();
        Task DeleteFrpConfigAsync(string id);
        Task<List<FrpConfigBase>> GetFrpConfigsAsync();
        Task GetFrpStatusAsync(string id);
        Task<IList<FrpProcess>> GetFrpStatusesAsync();
        Task<List<LogEntity>> GetLogsAsync(DateTime timeAfter);
        Task ModifyConfigAsync(FrpConfigBase config);
        Task RestartFrpAsync(string id);
        Task StartFrpAsync(string id);
        Task StopFrpAsync(string id);
    }
}
