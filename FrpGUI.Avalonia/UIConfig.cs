using System;
using System.IO;
using FrpGUI.Configs;
using System.ComponentModel;
using FrpGUI.Avalonia.Models;
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

    [JsonIgnore]
    public override string ConfigPath => Path.Combine(AppContext.BaseDirectory, "uiconfig.json");

    public RunningMode RunningMode
    {
        get => runningMode;
        set => this.SetValueAndNotify(ref runningMode, value, nameof(RunningMode));
    }

    public string ServerAddress { get; set; } = "http://localhost:5113";
    public string ServerToken { get; set; } = "";
    protected override JsonSerializerContext JsonSerializerContext => FrpAvaloniaSourceGenerationContext.Default;
}
