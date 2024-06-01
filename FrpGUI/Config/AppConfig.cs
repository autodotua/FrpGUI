using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Text.Json.Nodes;

namespace FrpGUI.Config
{
    public class AppConfig
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            //TypeInfoResolver = AppConfigSourceGenerationContext.Default,
            Converters = { new FrpConfigJsonConverter() },
            WriteIndented = true,
        };

        private static readonly object lockObj = new object();
        private static readonly string path = "config.json";
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
                                instance = JsonSerializer.Deserialize<AppConfig>(File.ReadAllBytes(path), jsonOptions);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("读取配置失败：" + ex.Message);
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

        public List<FrpConfigBase> FrpConfigs { get; set; } = new List<FrpConfigBase>();
        public string FrpConfigType { get; set; } = "TOML";
        public string RemoteControlAddress { get; set; } = "127.0.0.1";
        public bool RemoteControlEnable { get; set; } = true;
        public string RemoteControlPassword { get; set; } = "1234";
        public int RemoteControlPort { get; set; } = 12345;
        public bool ShowTrayIcon { get; set; } = true;
        public void Save()
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(this, jsonOptions);
            File.WriteAllBytes(path, bytes);
        }

        public class FrpConfigJsonConverter : JsonConverter<FrpConfigBase>
        {
            public override FrpConfigBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using JsonDocument doc = JsonDocument.ParseValue(ref reader);
                if (doc.RootElement.TryGetProperty("Type", out JsonElement typeElement) && typeElement.ValueKind == JsonValueKind.String)
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
                else if (doc.RootElement.TryGetProperty("Rules", out JsonElement rulesElement))
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
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppConfig))]
    internal partial class AppConfigSourceGenerationContext : JsonSerializerContext
    {
    }
}