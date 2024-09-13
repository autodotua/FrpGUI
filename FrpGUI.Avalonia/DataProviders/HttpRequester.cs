using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using FrpGUI.Avalonia.Models;
using System.Net.Http.Headers;

namespace FrpGUI.Avalonia.DataProviders
{
    public class HttpRequester
    {
        public string Token { get; private set; }

        private readonly HttpClient httpClient = new HttpClient();

        public void ReplaceToken(string newToken)
        {
            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", newToken);

        }

        protected string BaseApiUrl { get; set; } = "http://localhost:5113";

        public void Dispose()
        {
            httpClient.Dispose();
        }

        protected static async Task ProcessError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response == null)
                {
                    throw new Exception($"API请求失败（{(int)response.StatusCode}{response.StatusCode}）");
                }
                var message = await response.Content.ReadAsStringAsync();
                message = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception($"服务器处理错误（500）：{Environment.NewLine}{message}");
                }
                throw new Exception($"API请求失败（{(int)response.StatusCode}{response.StatusCode}）：{Environment.NewLine}{message}");
            }
        }

        protected async Task<HttpContent> GetAsync(string endpoint)
        {
            var response = await httpClient.GetAsync($"{BaseApiUrl}/{endpoint}");

            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            await ProcessError(response);
            throw new Exception();
        }

        protected Task<T> GetObjectAsync<T>(string endpoint, params (string Key, string Value)[] query) where T : class
        {
            var querys = query.Select(p => $"{p.Key}={p.Value}");
            return GetObjectAsync<T>(endpoint + "?" + string.Join('&', querys));
        }

        protected async Task<T> GetObjectAsync<T>(string endpoint)
        {
            using var responseStream = await (await GetAsync(endpoint)).ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(responseStream, JsonHelper.GetJsonOptions(FrpAvaloniaSourceGenerationContext.Default));
        }

        protected async Task PostAsync(string endpoint, object data = null)
        {
            var jsonContent = data == null ? null : new StringContent(JsonSerializer.Serialize(data, JsonHelper.GetJsonOptions(FrpAvaloniaSourceGenerationContext.Default)), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{BaseApiUrl}/{endpoint}", jsonContent);
            await ProcessError(response);
        }

        protected async Task<T> PostAsync<T>(string endpoint, object data = null)
        {
            var jsonContent = data == null ? null : new StringContent(JsonSerializer.Serialize(data, JsonHelper.GetJsonOptions(FrpAvaloniaSourceGenerationContext.Default)), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{BaseApiUrl}/{endpoint}", jsonContent);

            await ProcessError(response);
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), JsonHelper.GetJsonOptions(FrpAvaloniaSourceGenerationContext.Default));
        }
    }
}
