using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FrpGUI.Configs;
using FrpGUI.Models;
using System.Dynamic;
using FrpGUI.Avalonia.Models;
using System.Text.Json.Nodes;

namespace FrpGUI.Avalonia.DataProviders
{
    public class WebDataProvider : HttpRequester, IDataProvider
    {
        private const string AddClientEndpoint = "Config/FrpConfigs/Add/Client";
        private const string AddServerEndpoint = "Config/FrpConfigs/Add/Server";
        private const string DeleteFrpConfigsEndpoint = "Config/FrpConfigs/Delete";
        private const string FrpConfigsEndpoint = "Config/FrpConfigs";
        private const string FrpStatusEndpoint = "Process/Status";
        private const string LogsEndpoint = "Log/List";
        private const string ModifyConfigEndpoint = "Config/FrpConfigs/Modify";
        private const string RestartFrpEndpoint = "Process/Restart";
        private const string StartFrpEndpoint = "Process/Start";
        private const string StopFrpEndpoint = "Process/Stop";
        public Task<ClientConfig> AddClientAsync()
        {
            return PostAsync<ClientConfig>(AddClientEndpoint);
        }

        public Task<ServerConfig> AddServerAsync()
        {
            return PostAsync<ServerConfig>(AddServerEndpoint);
        }

        public Task DeleteFrpConfigAsync(string id)
        {
            return PostAsync($"{DeleteFrpConfigsEndpoint}/{id}");
        }


        public Task<List<FrpConfigBase>> GetFrpConfigsAsync()
        {
            return GetObjectAsync<List<FrpConfigBase>>(FrpConfigsEndpoint);
        }

        public Task GetFrpStatusAsync(string id)
        {
            return PostAsync($"{FrpStatusEndpoint}/{id}");
        }

        public async Task<IList<FrpProcess>> GetFrpStatusesAsync()
        {
            return await GetObjectAsync<IList<FrpProcess>>(FrpStatusEndpoint);
            //var content=await GetAsync(FrpStatusEndpoint);
            //var jarray = JsonNode.Parse(await content.ReadAsStreamAsync()) as JsonArray;
            //List<FrpProcess> fps = new List<FrpProcess>();
            //foreach (var jfp in jarray)
            //{
            //    var jconfig = jfp["config"] as JsonObject;
            //    if (jconfig["type"])
            //    FrpProcess fp = new FrpProcess();
            //}

            //return null;
        }

        public Task<List<LogEntity>> GetLogsAsync(DateTime timeAfter)
        {
            return GetObjectAsync<List<LogEntity>>(LogsEndpoint, ("timeAfter", timeAfter.ToString("o")));
        }

        public Task ModifyConfigAsync(FrpConfigBase config)
        {
            return PostAsync(ModifyConfigEndpoint, config);
        }

        public Task RestartFrpAsync(string id)
        {
            return PostAsync($"{RestartFrpEndpoint}/{id}");
        }

        public Task StartFrpAsync(string id)
        {
            return PostAsync($"{StartFrpEndpoint}/{id}");
        }

        public Task StopFrpAsync(string id)
        {
            return PostAsync($"{StopFrpEndpoint}/{id}");
        }


    }
}
