using FrpGUI.Avalonia.Models;
using FrpGUI.Configs;
using FrpGUI.Utils;
using FzLib;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FrpGUI.Avalonia;

[ObservableObject]
public partial class UIConfig : AppConfigBase
{
    [ObservableProperty]
    private RunningMode runningMode;

    [ObservableProperty]
    private bool showTrayIcon;

    public UIConfig() : base()
    {
    }

    public static UIConfig DefaultConfig { get; set; }

    [JsonIgnore]
    public override string ConfigPath => Path.Combine(AppContext.BaseDirectory, "uiconfig.json");

    public string ServerAddress { get; set; } = "http://localhost:5113";

    public string ServerToken { get; set; } = "";


    protected override JsonSerializerContext JsonSerializerContext => FrpAvaloniaSourceGenerationContext.Default;

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

    protected override T GetImpl<T>()
    {
        if (OperatingSystem.IsBrowser())
        {
            try
            {
                var json = JsInterop.GetLocalStorage("config");
                if (string.IsNullOrEmpty(json))
                {
                    if (DefaultConfig != null)
                    {
                        return DefaultConfig as T; //优先级2：默认配置。由于HttpClient不支持同步，所以DefaultConfig在Browser项目中进行了赋值
                    }

                    return new UIConfig() as T; //优先级3：新配置
                }

                //优先级1：LocalStorage配置
                return JsonSerializer.Deserialize<T>(JsInterop.GetLocalStorage("config"),
                                  JsonHelper.GetJsonOptions(JsonSerializerContext));
            }
            catch (Exception ex)
            {
                JsInterop.Alert("读取配置文件错误：" + ex.ToString());
                throw;
            }
        }
        else
        {
            return base.GetImpl<T>();
        }
    }
}