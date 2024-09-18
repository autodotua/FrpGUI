using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace FrpGUI.Avalonia;

[SupportedOSPlatform("browser")]
public partial class JsInterop
{
    [JSImport("setLocalStorage", "utils.js")]
    public static partial void SetLocalStorage(string key, string value);

    [JSImport("getLocalStorage", "utils.js")]
    public static partial string GetLocalStorage(string key);

    [JSImport("showAlert", "utils.js")]
    public static partial string Alert(string message);

    [JSImport("reload", "utils.js")]
    public static partial void Reload();

    [JSImport("getCurrentUrl", "utils.js")]
    public static partial string GetCurrentUrl();
}