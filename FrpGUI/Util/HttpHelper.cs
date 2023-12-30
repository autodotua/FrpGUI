using FrpGUI.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrpGUI
{
    public class HttpHelper : HttpHelperBase
    {
        public static HttpHelper Instance { get; } = new HttpHelper();

        public async Task<object> GetServerInfoAsync(ServerConfig server)
        {
            var response = await GetAsync(server, "serverinfo");
            return JsonConvert.DeserializeObject(response);
        }

        public async Task<List<object>> GetProxiesAsync(ServerConfig server, string type)
        {
            var response = await GetAsync(server, "proxy/" + type);
            var proxies = JObject.Parse(response)["proxies"] as JArray;
            List<object> result = new List<object>();
            foreach (JObject p in proxies)
            {
                p.Remove("conf");
                result.Add(JsonConvert.DeserializeObject(p.ToString()));
            }
            return result;
        }
    }
}