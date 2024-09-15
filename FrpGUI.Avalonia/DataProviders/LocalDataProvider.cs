using FrpGUI.Avalonia.Models;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static FzLib.Program.Runtime.SimplePipe;

namespace FrpGUI.Avalonia.DataProviders
{
    public class LocalDataProvider : IDataProvider
    {
        private readonly AppConfig configs;
        private readonly LoggerBase logger;
        private readonly FrpProcessCollection processes;
        public LocalDataProvider(AppConfig configs, FrpProcessCollection processes, LoggerBase logger)
        {
            this.configs = configs;
            this.processes = processes;
            this.logger = logger;
        }

        public Task<ClientConfig> AddClientAsync()
        {
            ClientConfig client = new ClientConfig();
            configs.FrpConfigs.Add(client);
            configs.Save();
            return Task.FromResult(client);
        }

        public Task<ServerConfig> AddServerAsync()
        {
            ServerConfig server = new ServerConfig();
            configs.FrpConfigs.Add(server);
            configs.Save();

            return Task.FromResult(server);
        }

        public Task DeleteFrpConfigAsync(string id)
        {
            return processes.RemoveFrpAsync(id);
        }

        public Task<List<FrpConfigBase>> GetFrpConfigsAsync()
        {
            return Task.FromResult(configs.FrpConfigs);
        }

        public Task<FrpStatusInfo> GetFrpStatusAsync(string id)
        {
            return Task.FromResult(new FrpStatusInfo(processes.GetOrCreateProcess(id)));
        }

        public Task<IList<FrpStatusInfo>> GetFrpStatusesAsync()
        {
            return Task.FromResult(processes.GetAll().Select(p => new FrpStatusInfo(p)).ToList() as IList<FrpStatusInfo>);
        }

        public Task<List<LogEntity>> GetLogsAsync(DateTime timeAfter)
        {
            return Task.FromResult((logger as Logger).GetLogs());
        }

        public Task ModifyConfigAsync(FrpConfigBase config)
        {
            var p = processes.GetOrCreateProcess(config.ID);
            if (p.Config.GetType() != config.GetType())
            {
                throw new ArgumentException("提供的配置与已有配置类型不同");
            }
            configs.Adapt(p.Config);
            configs.Save();
            return Task.CompletedTask;
        }

        public Task RestartFrpAsync(string id)
        {
            configs.Save();
            return processes.GetOrCreateProcess(id).RestartAsync();
        }

        public Task SetTokenAsync(string oldToken, string newToken)
        {
            throw new NotImplementedException();
        }

        public Task StartFrpAsync(string id)
        {
            configs.Save();
            return processes.GetOrCreateProcess(id).StartAsync();
        }

        public Task StopFrpAsync(string id)
        {
            return processes.GetOrCreateProcess(id).StopAsync();
        }

        public Task<TokenVerification> VerifyTokenAsync()
        {
            throw new NotImplementedException();
        }
    }
}
