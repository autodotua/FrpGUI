using FrpGUI.Configs;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigController : ControllerBase
{
    private readonly AppConfig config;

    public ConfigController(AppConfig config)
    {
        this.config = config;
    }

    [HttpGet("FrpConfigs")]
    public List<FrpConfigBase> GetFrpConfigList()
    {
        return config.FrpConfigs;
    }
}
//[ApiController]
//[Route("[controller]")]
//public class AppConfigController : ControllerBase
//{
//    //[HttpGet("RemoteControlAddress")]
//    //public string GetRemoteControlAddress()
//    //{
//    //    return AppConfig.Instance.RemoteControlAddress;
//    //}

//    //[HttpPost("RemoteControlAddress")]
//    //public void SetRemoteControlAddress(string value)
//    //{
//    //    AppConfig.Instance.RemoteControlAddress = value;
//    //}

//    //[HttpGet("RemoteControlEnable")]
//    //public bool GetRemoteControlEnable()
//    //{
//    //    return AppConfig.Instance.RemoteControlEnable;
//    //}

//    //[HttpPost("RemoteControlEnable")]
//    //public void SetRemoteControlEnable(bool value)
//    //{
//    //    AppConfig.Instance.RemoteControlEnable = value;
//    //}

//    //[HttpGet("RemoteControlPassword")]
//    //public string GetRemoteControlPassword()
//    //{
//    //    // 注意：直接通过API返回密码可能带来安全风险，请谨慎处理。
//    //    return AppConfig.Instance.RemoteControlPassword;
//    //}

//    //[HttpPost("RemoteControlPassword")]
//    //public void SetRemoteControlPassword(string value)
//    //{
//    //    // 验证密码强度或其他安全措施
//    //    AppConfig.Instance.RemoteControlPassword = value;
//    //}

//    //[HttpGet("RemoteControlPort")]
//    //public int GetRemoteControlPort()
//    //{
//    //    return AppConfig.Instance.RemoteControlPort;
//    //}

//    //[HttpPost("RemoteControlPort")]
//    //public void SetRemoteControlPort(int value)
//    //{
//    //    AppConfig.Instance.RemoteControlPort = value;
//    //}

//    //[HttpGet("ShowTrayIcon")]
//    //public bool GetShowTrayIcon()
//    //{
//    //    return AppConfig.Instance.ShowTrayIcon;
//    //}

//    //[HttpPost("ShowTrayIcon")]
//    //public void SetShowTrayIcon(bool value)
//    //{
//    //    AppConfig.Instance.ShowTrayIcon = value;
//    //}
//}
