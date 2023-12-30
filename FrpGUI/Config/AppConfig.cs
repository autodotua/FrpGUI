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
    public class AppConfig : IJsonSerializable
    {
        private static readonly string path = "config.json";

        private static AppConfig instance;

        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
        };

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
                if (instance == null)
                {
                    instance = new AppConfig();
                    if (File.Exists(path))
                    {
                        try
                        {
                            ConvertOldConfigJson();
                            instance.TryLoadFromJsonFile(path, jsonSettings);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    if (instance.FrpConfigs.Count == 0)
                    {
                        instance.FrpConfigs.Add(new ServerConfig());
                        instance.FrpConfigs.Add(new ClientConfig());
                    }
                }
                return instance;
            }
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

        public void Save()
        {
            this.Save(path, jsonSettings);
        }

        public string AdminAddress { get; set; } = "127.0.0.1";
        public string AdminPassword { get; set; } = "";
        public int AdminPort { get; set; } = 12345;
        public string FrpConfigType { get; set; } = "TOML"; //TOML、INI
        public List<FrpConfigBase> FrpConfigs { get; set; } = new List<FrpConfigBase>();
    }
}