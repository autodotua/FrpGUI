using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using FrpGUI.Models;

namespace FrpGUI.Configs
{
    public class AppConfig
    {
        private static readonly object lockObj = new object();
        public static readonly string ConfigPath = "config.json";
        private static AppConfig instance;

        public AppConfig() : base()
        {
        }
        /// <summary>
        /// 配置类单例
        /// </summary>

        public List<FrpConfigBase> FrpConfigs { get; set; } = new List<FrpConfigBase>();

        public string Token { get; set; }

        public void Save()
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(this, JsonHelper.GetJsonOptions(AppConfigSourceGenerationContext.Default));
            File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, ConfigPath), bytes);
        }


    }
    public class FrpConfigJsonConverter : JsonConverter<FrpConfigBase>
    {
        public override FrpConfigBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if ((doc.RootElement.TryGetProperty("Type", out JsonElement typeElement) || doc.RootElement.TryGetProperty("type", out typeElement))
                && typeElement.ValueKind == JsonValueKind.String)
            {
                char typeChar = typeElement.GetString()[0];
                return typeChar switch
                {
                    'c' => JsonSerializer.Deserialize<ClientConfig>(doc.RootElement.GetRawText(), options),
                    's' => JsonSerializer.Deserialize<ServerConfig>(doc.RootElement.GetRawText(), options),
                    _ => throw new NotImplementedException(),
                };
            }
            //老版本的JSON配置文件没有Type属性
            else if (doc.RootElement.TryGetProperty("Rules", out JsonElement rulesElement) || doc.RootElement.TryGetProperty("rules", out rulesElement))
            {
                return JsonSerializer.Deserialize<ClientConfig>(doc.RootElement.GetRawText(), options);
            }
            else
            {
                return JsonSerializer.Deserialize<ServerConfig>(doc.RootElement.GetRawText(), options);
            }

        }

        public override void Write(Utf8JsonWriter writer, FrpConfigBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppConfig))]
    internal partial class AppConfigSourceGenerationContext : JsonSerializerContext
    {
    }
}
