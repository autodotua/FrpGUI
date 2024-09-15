using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FrpGUI.Avalonia.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(FrpStatusInfo))]
[JsonSerializable(typeof(UIConfig))]
[JsonSerializable(typeof(AppConfigBase<UIConfig>))]
[JsonSerializable(typeof(LogEntity))]
[JsonSerializable(typeof(TokenVerification))]
[JsonSerializable(typeof(List<LogEntity>))]
[JsonSerializable(typeof(IList<FrpStatusInfo>))]
internal partial class FrpAvaloniaSourceGenerationContext : JsonSerializerContext
{
}
