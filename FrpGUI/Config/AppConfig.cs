using FzLib.DataStorage.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Serialization;

namespace FrpGUI.Config
{
    public class AppConfig : IJsonSerializable
    {
        private static readonly string path = "configs.json";

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
                if (instance == null)
                {
                    instance = new AppConfig();
                    try
                    {
                        instance.TryLoadFromJsonFile(path);
                    }
                    catch (Exception ex)
                    {
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

        public void Save()
        {
            this.Save(path, new JsonSerializerSettings().SetIndented());
        }

        public string AdminAddress { get; set; } = "127.0.0.1";
        public string AdminPassword { get; set; } = "";
        public int AdminPort { get; set; } = 12345;
        public List<FrpConfigBase> FrpConfigs { get; set; } = new List<FrpConfigBase>();
    }
}