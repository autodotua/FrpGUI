﻿using FrpGUI.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrpGUI
{
    public class HttpHelperBase
    {
        protected Task<string> GetAsync(ServerConfig server, string url, Dictionary<string, string> query = null)
        {
            return GetOrPostAndReturnJsonAsync(server, url, query, (c, u) => c.GetAsync(u));
        }

        //protected async Task<byte[]> GetFileAsync(string url, Dictionary<string, string> query = null)
        //{
        //    var content = await GetOrPostAsync(url, query, (c, u) => c.GetAsync(u));
        //    JsonResponse r = null;
        //    try
        //    {
        //        var json = await content.ReadAsStringAsync();

        //        r = new JsonResponse(json);
        //    }
        //    catch
        //    {
        //    }

        //    if (r != null)
        //    {
        //        if (r.Success)
        //        {
        //            Debug.Assert(false);
        //        }
        //        throw new Exception(r.Message);
        //    }
        //    var bytes = await content.ReadAsByteArrayAsync();
        //    return new Response<byte[]>()
        //    {
        //        Success = true,
        //        Data = bytes
        //    };
        //}

        protected async Task<string> PostAsync(ServerConfig server, string url, object body, Dictionary<string, string> query = null)
        {
            string json = JsonSerializer.Serialize(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await GetOrPostAndReturnJsonAsync(server, url, query, (c, u) => c.PostAsync(u, httpContent));
            return result;
        }

        //protected async Task<string> PostAsync(string url, byte[] data, Dictionary<string, string> query = null)
        //{
        //    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.Ticks.ToString("X")))
        //    {
        //        content.Add(new StreamContent(new MemoryStream(data)), "file", "upload.wav");
        //        result = await GetOrPostAndReturnJsonAsync(url, query, (c, u) => c.PostAsync(u, content));
        //    }
        //    return result;
        //}

        protected async Task<string> GetOrPostAndReturnJsonAsync(ServerConfig server,
            string url,
            Dictionary<string, string> query,
            Func<HttpClient, Uri, Task<HttpResponseMessage>> getResponse)
        {
            try
            {
                var httpContent = await GetOrPostAsync(server, url, query, getResponse);
                return await httpContent.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw;
            };
        }

        private async Task<HttpContent> GetOrPostAsync(ServerConfig server,
            string url,
            Dictionary<string, string> query,
            Func<HttpClient, Uri, Task<HttpResponseMessage>> getResponse)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                           Encoding.UTF8.GetBytes($"{server.DashBoardUsername}:{server.DashBoardPassword}")));
                    var fullUrl = new Uri($"http://localhost:{server.DashBoardPort}/api/{url}");

                    HttpResponseMessage response = await getResponse(httpClient, fullUrl);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception("接口访问失败：" + response.StatusCode.ToString());
                    }
                    var result = response.Content;
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            };
        }
    }
}