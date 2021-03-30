using FzLib.DataStorage.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Serialization;

namespace FrpGUI
{
    public class Config : JsonSerializationBase
    {
        public Config() : base()
        {
        }

        private static Config instance;

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = OpenOrCreate<Config>(settings: new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                    if (instance.FrpConfigs.Count == 0)
                    {
                        instance.FrpConfigs.Add(new ServerConfig());
                        instance.FrpConfigs.Add(new ClientConfig());
                    }
                }

                return instance;
            }
        }

        public List<FrpConfigBase> FrpConfigs { get; set; } = new List<FrpConfigBase>();
        public string AdminAddress { get; set; } = "127.0.0.1";
        public int AdminPort { get; set; } = 12345;
        public string AdminPassword { get; set; } = "";
    }
}