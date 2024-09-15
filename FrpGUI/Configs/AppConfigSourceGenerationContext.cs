using System.Text.Json.Serialization;

namespace FrpGUI.Configs
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppConfig))]
    [JsonSerializable(typeof(AppConfigBase<AppConfig>))]
    internal partial class AppConfigSourceGenerationContext : JsonSerializerContext
    {
    }
}
