using System;
using System.IO;
using FrpGUI.Configs;
using System.Collections.Generic;
using System.Text.Json;
using FrpGUI.Avalonia.Models;

namespace FrpGUI.Avalonia;

public class AppConfig
{
    public static readonly string ConfigPath = "uiconfig.json";

    public AppConfig() : base()
    {
    }

    public string Token { get; set; }

    public void Save()
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(this, JsonHelper.GetJsonOptions(FrpAvaloniaSourceGenerationContext.Default));
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, ConfigPath), bytes);
    }


}
