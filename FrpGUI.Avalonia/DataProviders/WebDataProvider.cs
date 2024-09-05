﻿using System;
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
        private const string AddClientEndpoint = "Config/Add/Client";
        private const string AddServerEndpoint = "Config/Add/Server";
        private const string ModifyConfigEndpoint = "Config/Modify";
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
            return GetAsync<List<FrpConfigBase>>(FrpConfigsEndpoint);
        }
        public Task<List<LogEntity>> GetLogsAsync(DateTime timeAfter)
        {
            return GetAsync<List<LogEntity>>(LogsEndpoint, ("timeAfter", timeAfter.ToString()));
        }

        public Task RestartFrpAsync(string id)
        {
            return PostAsync(RestartFrpEndpoint);
        }

        public Task StartFrpAsync(string id)
        {
            return PostAsync(StartFrpEndpoint);
        }

        public Task StopFrpAsync(string id)
        {
            return PostAsync(StopFrpEndpoint);
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

        private Task<T> GetAsync<T>(string endpoint, params (string Key, string Value)[] query) where T : class
        {
            var querys = query.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}");
            return GetAsync<T>(endpoint + "?" + string.Join('&', querys));
        }

        private async Task<T> GetAsync<T>(string endpoint) where T : class
        {
            var response = await HttpClient.GetAsync($"{BaseApiUrl}/{endpoint}");

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, AppConfig.JsonOptions);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                throw new Exception($"API请求失败，状态码：{response.StatusCode}");
            }
        }

        private async Task PostAsync(string endpoint, object data = null)
        {
            var jsonContent = data == null ? null : new StringContent(JsonSerializer.Serialize(data, AppConfig.JsonOptions), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync($"{BaseApiUrl}/{endpoint}", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API请求失败，状态码：{response.StatusCode}");
            }
        }

        private async Task<T> PostAsync<T>(string endpoint, object data = null)
        {
            var jsonContent = data == null ? null : new StringContent(JsonSerializer.Serialize(data, AppConfig.JsonOptions), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync($"{BaseApiUrl}/{endpoint}", jsonContent);
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), AppConfig.JsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API请求失败，状态码：{response.StatusCode}");
            }
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