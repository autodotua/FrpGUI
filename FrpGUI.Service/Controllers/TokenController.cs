using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Service.Models;
using FrpGUI.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrpGUI.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : FrpControllerBase
{
    public TokenController(AppConfig config, LoggerBase logger) : base(config, logger)
    {
    }

    [HttpGet]
    public TokenVerification Verify()
    {
        if (string.IsNullOrEmpty(configs.Token))
        {
            return TokenVerification.NeedSet;
        }
        var authorization = HttpContext.Request.Headers.Authorization.ToString();
        if (authorization == configs.Token)
        {
            return TokenVerification.OK;
        }
        return TokenVerification.NotEqual;
    }

    [HttpPost]
    public void Set(string oldToken, string newToken)
    {
        if (configs.Token != oldToken)
        {
            if (!string.IsNullOrWhiteSpace(configs.Token) || !string.IsNullOrWhiteSpace(oldToken))
            {
                throw new ArgumentException("旧密码错误", nameof(oldToken));
            }
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(newToken, nameof(newToken));
        configs.Token = newToken;
        configs.Save();
    }
}
