using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using FrpGUI.Models;

namespace FrpGUI.Configs
{
    public class AppConfig : AppConfigBase<AppConfig>
    {
        public override string ConfigPath => Path.Combine(AppContext.BaseDirectory, "config.json");
        public List<FrpConfigBase> FrpConfigs { get; set; } = new List<FrpConfigBase>();
        public string Token { get; set; }
        protected override JsonSerializerContext JsonSerializerContext => AppConfigSourceGenerationContext.Default;
        protected override void OnLoaded()
        {
            if (FrpConfigs.Count == 0)
            {
                FrpConfigs.Add(new ServerConfig());
                FrpConfigs.Add(new ClientConfig());
            }
        }
    }
}
