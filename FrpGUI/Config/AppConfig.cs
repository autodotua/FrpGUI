using FzLib.DataStorage.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Serialization;

namespace FrpGUI.Config
{
    public class AppConfig
    {
        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
        };

        private static readonly string path = "config.json";

        private static readonly object lockObj = new object();

        private static AppConfig instance;

        public AppConfig() : base()
        {
        }
        /// <summary>
        /// 配置类单例
        /// </summary>
        public static AppConfig Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new AppConfig();
                        if (File.Exists(path))
                        {
                            try
                            {
                                ConvertOldConfigJson();
                                instance = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(path), jsonSettings);
                            }
                            catch (Exception ex)
                            {
                                instance = new AppConfig();
                            }
                        }
                        if (instance.FrpConfigs.Count == 0)
                        {
                            instance.FrpConfigs.Add(new ServerConfig());
                            instance.FrpConfigs.Add(new ClientConfig());
                        }
                    }
                }
                return instance;
            }
        }

        public string AdminAddress { get; set; } = "127.0.0.1";

        public string AdminPassword { get; set; } = "";

        public int AdminPort { get; set; } = 12345;

        public List<FrpConfigBase> FrpConfigs { get; set; } = new List<FrpConfigBase>();

        public string FrpConfigType { get; set; } = "TOML";

        public void Save()
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, jsonSettings));
        }

        private static void ConvertOldConfigJson()
        {
            JObject json = JObject.Parse(File.ReadAllText(path));
            var configs = json["FrpConfigs"] as JArray;
            if (json["$type"] != null)
            {
                return;
            }
            foreach (var cfg in configs)
            {
                if (cfg["Port"] != null)
                {
                    cfg["$type"] = "FrpGUI.Config.ServerConfig, FrpGUI";
                }
                else
                {
                    cfg["$type"] = "FrpGUI.Config.ClientConfig, FrpGUI";
                }
            }
            File.WriteAllText(path, json.ToString(Formatting.Indented));
        }
        //TOML、INI
    }
}