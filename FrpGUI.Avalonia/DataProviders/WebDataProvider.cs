using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FrpGUI.Configs;
using FrpGUI.Models;
using System.Dynamic;
using System.Text.Json.Nodes;
using FrpGUI.Enums;
using System.Net;
using System.Linq;
using FrpGUI.Avalonia.ViewModels;
using System.Threading;
using FzLib.Models;

namespace FrpGUI.Avalonia.DataProviders
{
    public class WebDataProvider : HttpRequester, IDataProvider
    {
        private const string AddClientEndpoint = "Config/FrpConfigs/Add/Client";
        private const string AddServerEndpoint = "Config/FrpConfigs/Add/Server";
        private const string DeleteFrpConfigsEndpoint = "Config/FrpConfigs/Delete";
        private const string FrpConfigsEndpoint = "Config/FrpConfigs";
        private const string FrpStatusEndpoint = "Process/Status";
        private const string KillProcessEndpoint = "Process/Kill";
        private const string LogsEndpoint = "Log/List";
        private const string ModifyConfigEndpoint = "Config/FrpConfigs/Modify";
        private const string RestartFrpEndpoint = "Process/Restart";
        private const string StartFrpEndpoint = "Process/Start";
        private const string StopFrpEndpoint = "Process/Stop";
        private const string SystemProcessesEndpoint = "Process/All";
        private const string TokenEndpoint = "Token";
        private readonly UIConfig config;
        private readonly LocalLogger logger;
        private PeriodicTimer timer;

        private List<(string Name, Func<Task> task)> timerTasks = new List<(string Name, Func<Task> task)>();

        public WebDataProvider(UIConfig config, LocalLogger logger) : base(config)
        {
            this.config = config;
            this.logger = logger;
            StartTimer();
        }

        public Task<ClientConfig> AddClientAsync()
        {
            return PostAsync<ClientConfig>(AddClientEndpoint);
        }

        public Task<ServerConfig> AddServerAsync()
        {
            return PostAsync<ServerConfig>(AddServerEndpoint);
        }

        public void AddTimerTask(string name, Func<Task> task)
        {
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
            ArgumentNullException.ThrowIfNull(task, nameof(task));
            timerTasks.Add((name, task));
        }

        public Task DeleteFrpConfigAsync(string id)
        {
            return PostAsync($"{DeleteFrpConfigsEndpoint}/{id}");
        }

        public Task<List<FrpConfigBase>> GetFrpConfigsAsync()
        {
            return GetObjectAsync<List<FrpConfigBase>>(FrpConfigsEndpoint);
        }

        public Task<FrpStatusInfo> GetFrpStatusAsync(string id)
        {
            return PostAsync<FrpStatusInfo>($"{FrpStatusEndpoint}/{id}");
        }

        public async Task<IList<FrpStatusInfo>> GetFrpStatusesAsync()
        {
            var result = await GetObjectAsync<IList<FrpStatusInfo>>(FrpStatusEndpoint);
            return result;//.Select(p => new FrpStatusInfo(p)).ToList();
        }

        public Task<List<LogEntity>> GetLogsAsync(DateTime timeAfter)
        {
            return GetObjectAsync<List<LogEntity>>(LogsEndpoint, ("timeAfter", timeAfter.ToString("o")));
        }

        public Task<List<ProcessInfo>> GetSystemProcesses()
        {
            return GetObjectAsync<List<ProcessInfo>>(SystemProcessesEndpoint);
        }

        public Task KillProcess(int id)
        {
            return PostAsync($"{KillProcessEndpoint}/{id}");
        }

        public Task ModifyConfigAsync(FrpConfigBase config)
        {
            return PostAsync(ModifyConfigEndpoint, config);
        }

        public Task RestartFrpAsync(string id)
        {
            return PostAsync($"{RestartFrpEndpoint}/{id}");
        }

        public Task SetTokenAsync(string oldToken, string newToken)
        {
            return PostAsync<TokenVerification>($"{TokenEndpoint}?oldToken={WebUtility.UrlEncode(oldToken)}&newToken={WebUtility.UrlEncode(newToken)}");
        }

        public Task StartFrpAsync(string id)
        {
            return PostAsync($"{StartFrpEndpoint}/{id}");
        }

        public Task StopFrpAsync(string id)
        {
            return PostAsync($"{StopFrpEndpoint}/{id}");
        }

        public Task<TokenVerification> VerifyTokenAsync()
        {
            return GetObjectAsync<TokenVerification>(TokenEndpoint);
        }

        private async void StartTimer()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            while (await timer.WaitForNextTickAsync())
            {
                foreach (var (name, task) in timerTasks.ToList())
                {
                    try
                    {
                        await task.Invoke();
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"执行定时任务“{name}”失败", null, ex);
                    }
                }
            }
        }
    }
}
