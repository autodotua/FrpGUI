using FrpGUI.Models;
using System.Text.Json.Serialization;

namespace FrpGUI.Configs
{
    public class AppConfig : AppConfigBase
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