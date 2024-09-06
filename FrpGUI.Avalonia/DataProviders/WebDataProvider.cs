using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using FrpGUI.Configs;
using System.Net.Http;
using FrpGUI.Models;
using System.Dynamic;
using FrpGUI.Avalonia.Models;
using System.Text.Json.Nodes;

namespace FrpGUI.Avalonia.DataProviders
{
    public class WebDataProvider : IDataProvider
    {
        private const string BaseApiUrl = "http://localhost:5113";
        private const string DeleteFrpConfigsEndpoint = "Config/FrpConfigs/Delete";
        private const string FrpConfigsEndpoint = "Config/FrpConfigs";
        private const string LogsEndpoint = "Log/List";
        private const string RestartFrpEndpoint = "Process/Restart";
        private const string StartFrpEndpoint = "Process/Start";
        private const string StopFrpEndpoint = "Process/Stop";
        private const string FrpStatusEndpoint = "Process/Status";
        private const string AddClientEndpoint = "Config/FrpConfigs/Add/Client";
        private const string AddServerEndpoint = "Config/FrpConfigs/Add/Server";
        private const string ModifyConfigEndpoint = "Config/FrpConfigs/Modify";
        private readonly HttpClient HttpClient = new HttpClient();
        public Task DeleteFrpConfigAsync(string id)
        {
            return PostAsync(DeleteFrpConfigsEndpoint, id);
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        public Task<List<FrpConfigBase>> GetFrpConfigsAsync()
        {
            return GetObjectAsync<List<FrpConfigBase>>(FrpConfigsEndpoint);
        }
        public Task<List<LogEntity>> GetLogsAsync(DateTime timeAfter)
        {
            return GetObjectAsync<List<LogEntity>>(LogsEndpoint, ("timeAfter", timeAfter.ToString("o")));
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

        public Task<ClientConfig> AddClientAsync()
        {
            return PostAsync<ClientConfig>(AddClientEndpoint);
        }

        public Task<ServerConfig> AddServerAsync()
        {
            return PostAsync<ServerConfig>(AddServerEndpoint);
        }

        public Task ModifyConfigAsync(FrpConfigBase config)
        {
            return PostAsync(ModifyConfigEndpoint, config);
        }

        private async Task DeleteAsync(string endpoint)
        {
            var response = await HttpClient.DeleteAsync($"{BaseApiUrl}/{endpoint}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API请求失败，状态码：{response.StatusCode}");
            }
        }

        private Task<T> GetObjectAsync<T>(string endpoint, params (string Key, string Value)[] query) where T : class
        {
            var querys = query.Select(p => $"{p.Key}={p.Value}");
            return GetObjectAsync<T>(endpoint + "?" + string.Join('&', querys));
        }

        private async Task<T> GetObjectAsync<T>(string endpoint) where T : class
        {
            using var responseStream = await (await GetAsync(endpoint)).ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(responseStream, AppConfig.JsonOptions);
        }

        private async Task<HttpContent> GetAsync(string endpoint)
        {
            var response = await HttpClient.GetAsync($"{BaseApiUrl}/{endpoint}");

            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                var message = response.Content == null ? "（无返回信息）" : await response.Content.ReadAsStringAsync();
                message = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];
                throw new Exception($"API请求失败（{response.StatusCode}）：{message}");
            }
        }

        private async Task PostAsync(string endpoint, object data = null)
        {
            var jsonContent = data == null ? null : new StringContent(JsonSerializer.Serialize(data, AppConfig.JsonOptions), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync($"{BaseApiUrl}/{endpoint}", jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                var message = response.Content == null ? "（无返回信息）" : await response.Content.ReadAsStringAsync();
                message = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];
                throw new Exception($"API请求失败（{response.StatusCode}）：{message}");
            }
        }

        private async Task<T> PostAsync<T>(string endpoint, object data = null)
        {
            var jsonContent = data == null ? null : new StringContent(JsonSerializer.Serialize(data, AppConfig.JsonOptions), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync($"{BaseApiUrl}/{endpoint}", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var message = response.Content == null ? "（无返回信息）" : await response.Content.ReadAsStringAsync();
                message = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];
                throw new Exception($"API请求失败（{response.StatusCode}）：{message}");
            }
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), AppConfig.JsonOptions);
        }

        private async Task PutAsync(string endpoint, object data)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await HttpClient.PutAsync($"{BaseApiUrl}/{endpoint}", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API请求失败，状态码：{response.StatusCode}");
            }
        }
    }
}
