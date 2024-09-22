namespace FrpGUI.WebAPI.Services
{
    public class WebConfigService(IConfiguration config)
    {
        public bool ServerOnly()
        {
            return bool.Parse(config["ServerOnly"]);
        }

        public void ThrowIfServerOnly()
        {
            if (ServerOnly())
            {
                throw new StatusBasedException("当前配置不允许使用frp客户端", System.Net.HttpStatusCode.Forbidden);
            }
        }
    }
}
