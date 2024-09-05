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

namespace FrpGUI.Avalonia.DataProviders
{
    public class WebDataProvider
    {
        private readonly HttpClient HttpClient = new HttpClient();
        private const string BaseApiUrl = "https://localhost:5113";

        private const string FrpConfigsEndpoint = "Config/FrpConfigs";


        public Task<List<FrpConfigBase>> GetFrpConfigsAsync()
        {
            return GetAsync<List<FrpConfigBase>>(FrpConfigsEndpoint);
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

        private async Task PostAsync(string endpoint, object data)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(data, AppConfig.JsonOptions), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync($"{BaseApiUrl}/{endpoint}", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API请求失败，状态码：{response.StatusCode}");
            }
        }

        private async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(data, AppConfig.JsonOptions), Encoding.UTF8, "application/json");
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

        private async Task DeleteAsync(string endpoint)
        {
            var response = await HttpClient.DeleteAsync($"{BaseApiUrl}/{endpoint}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API请求失败，状态码：{response.StatusCode}");
            }
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

    }
}
