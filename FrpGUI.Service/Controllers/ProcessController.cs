using FrpGUI.Configs;
using FrpGUI.Service.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;

namespace FrpGUI.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ProcessController : ControllerBase
{
    private readonly AppConfig config;

    private static Dictionary<FrpConfigBase, FrpProcess> frpProcesses = new Dictionary<FrpConfigBase, FrpProcess>();
    public ProcessController(AppConfig config)
    {
        this.config = config;
    }

    private FrpProcess GetOrCreateProcess(string id)
    {
        var frp = config.FrpConfigs.FirstOrDefault(p => p.ID == id) ?? throw new ArgumentException($"找不到ID为{id}的配置");
        if (frpProcesses.TryGetValue(frp, out FrpProcess process))
        {
            return process;
        }
        process = new FrpProcess(frp);
        return process;
    }

    [HttpGet("Start")]
    public Task StartAsync(string id)
    {
        return GetOrCreateProcess(id).StartAsync();
    }

    [HttpGet("Stop")]
    public Task StopAsync(string id)
    {
        return GetOrCreateProcess(id).StopAsync();
    }

    [HttpGet("Restart")]
    public Task RestartAsync(string id)
    {
        return GetOrCreateProcess(id).RestartAsync();
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
