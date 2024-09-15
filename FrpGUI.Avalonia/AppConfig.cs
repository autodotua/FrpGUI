using System;
using System.IO;
using FrpGUI.Configs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using FrpGUI.Avalonia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
using FzLib;

namespace FrpGUI.Avalonia;

public class UIConfig : AppConfigBase<UIConfig>, INotifyPropertyChanged
{
    private RunningMode runningMode;

    public UIConfig() : base()
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public override string ConfigPath => Path.Combine(AppContext.BaseDirectory, "uiconfig.json");
    public RunningMode RunningMode
    {
        get => runningMode;
        set => this.SetValueAndNotify(ref runningMode, value, nameof(RunningMode));
    }

    public string Token { get; set; }
    protected override JsonSerializerContext JsonSerializerContext => FrpAvaloniaSourceGenerationContext.Default;
    private string ServerIP { get; set; } = "localhost";

    private short ServerPort { get; set; } = 5113;

    private string ServerToken { get; set; } = "";
}

public enum RunningMode
{
    [Description("单机模式")]
    Singleton,
    [Description("服务模式")]
    Service
}