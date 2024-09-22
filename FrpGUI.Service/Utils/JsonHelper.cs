using FrpGUI.Configs;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace FrpGUI.Utils;

public static class JsonHelper
{
    public static JsonSerializerOptions GetJsonOptions(JsonSerializerContext typeInfoResolver)
    {
        return new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            TypeInfoResolver = typeInfoResolver,
            Converters = { new FrpConfigJsonConverter() },
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };
    }

    public static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            Converters = { new FrpConfigJsonConverter() },
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };
    }
}