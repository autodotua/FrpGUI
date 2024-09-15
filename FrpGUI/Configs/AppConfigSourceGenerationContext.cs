using System.Text.Json.Serialization;

namespace FrpGUI.Configs
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppConfig))]
    internal partial class AppConfigSourceGenerationContext : JsonSerializerContext
    {
    }
}
