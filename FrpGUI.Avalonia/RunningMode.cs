using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FrpGUI.Avalonia;

public enum RunningMode
{
    [Description("单机模式")]
    Singleton,
    [Description("服务模式")]
    Service
}