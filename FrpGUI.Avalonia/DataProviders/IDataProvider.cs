using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrpGUI.Avalonia.Models;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;

namespace FrpGUI.Avalonia.DataProviders
{
    public interface IDataProvider
    {
        Task<ClientConfig> AddClientAsync();
        Task<ServerConfig> AddServerAsync();
        Task DeleteFrpConfigAsync(string id);
        Task<List<FrpConfigBase>> GetFrpConfigsAsync();
        Task<FrpStatusInfo> GetFrpStatusAsync(string id);
        Task<IList<FrpStatusInfo>> GetFrpStatusesAsync();
        Task<List<LogEntity>> GetLogsAsync(DateTime timeAfter);
        Task ModifyConfigAsync(FrpConfigBase config);
        Task RestartFrpAsync(string id);
        Task SetTokenAsync(string oldToken, string newToken);
        Task StartFrpAsync(string id);
        Task StopFrpAsync(string id);
        Task<TokenVerification> VerifyTokenAsync();
    }
}
