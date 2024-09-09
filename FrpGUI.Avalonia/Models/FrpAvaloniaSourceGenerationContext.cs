using FrpGUI.Configs;
using FrpGUI.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FrpGUI.Avalonia.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(FrpProcess))]
[JsonSerializable(typeof(AppConfig))]
[JsonSerializable(typeof(LogEntity))]
[JsonSerializable(typeof(List<LogEntity>))]
[JsonSerializable(typeof(IList<FrpProcess>))]
internal partial class FrpAvaloniaSourceGenerationContext : JsonSerializerContext
{
}
