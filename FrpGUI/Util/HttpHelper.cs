using FrpGUI.Config;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FrpGUI
{
    public class HttpHelper : HttpHelperBase
    {
        public static HttpHelper Instance { get; } = new HttpHelper();

        public async Task<IDictionary<string, string>> GetServerInfoAsync(ServerConfig server)
        {
            var response = await GetAsync(server, "serverinfo");
            var jobj = JsonNode.Parse(response) as JsonObject;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in jobj)
            {
                dic.Add(item.Key, item.Value?.ToString() ?? "");
            }
            return dic;
        }
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        public async Task<List<ProxyStatusInfo>> GetProxiesAsync(ServerConfig server, string type)
        {
            var response = await GetAsync(server, "proxy/" + type);
            var proxies = JsonNode.Parse(response)["proxies"] as JsonArray;

            return proxies.Deserialize<List<ProxyStatusInfo>>(jsonOptions);
        }
    }


    public class ProxyStatusInfo
    {
        public string Name { get; set; }
        public string ClientVersion { get; set; }
        public long TodayTrafficIn { get; set; }
        public long TodayTrafficOut { get; set; }
        public long CurConns { get; set; }
        public string LastStartTime { get; set; }
        public string LastCloseTime { get; set; }
        public string Status { get; set; }
    }
}