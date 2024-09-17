using System;
using System.IO;
using FrpGUI.Configs;
using System.ComponentModel;
using FrpGUI.Avalonia.Models;
using System.Text.Json.Serialization;
using FzLib;
using System.Text.Json;

namespace FrpGUI.Avalonia;

public class UIConfig : AppConfigBase, INotifyPropertyChanged
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
    protected override T GetImpl<T>()
    {
        if (OperatingSystem.IsBrowser())
        {
            try
            {
                var json = JsInterop.GetLocalStorage("config");
                if (string.IsNullOrEmpty(json))
                {
                    return new UIConfig() as T;
                }
                return JsonSerializer.Deserialize<T>(JsInterop.GetLocalStorage("config"),
                                  JsonHelper.GetJsonOptions(JsonSerializerContext));
            }
            catch (Exception ex)
            {
                JsInterop.Alert("读取配置文件错误：" + ex.Message);
                throw;
            }
        }
        else
        {
            return base.GetImpl<T>();
        }
    }
    public override void Save()
    {
        if (OperatingSystem.IsBrowser())
        {
            var json = JsonSerializer.Serialize(this, typeof(UIConfig), JsonHelper.GetJsonOptions(JsonSerializerContext));
            JsInterop.SetLocalStorage("config", json);
        }
        else
        {
            base.Save();
        }
    }
}
