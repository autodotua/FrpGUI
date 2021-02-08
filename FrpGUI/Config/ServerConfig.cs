using System.Text;

namespace FrpGUI
{
    public class ServerConfig : IToIni
    {
        public string Token { get; set; }
        public ushort Port { get; set; } = 7000;
        public ushort DashBoardPort { get; set; } = 7500;
        public string DashBoardUsername { get; set; } = "admin";
        public string DashBoardPassword { get; set; } = "admin";
        public ushort? HttpPort { get; set; }
        public ushort? HttpsPort { get; set; }

        public string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[common]").AppendLine();
            str.Append("bind_port = ").Append(Port).AppendLine();
            str.Append("dashboard_port = ").Append(DashBoardPort).AppendLine();
            str.Append("dashboard_user = ").Append(DashBoardUsername).AppendLine();
            str.Append("dashboard_pwd = ").Append(DashBoardPassword).AppendLine();
            if (HttpPort.HasValue && HttpPort.Value > 0)
            {
                str.Append("vhost_http_port = ").Append(HttpPort.Value).AppendLine();
            }
            if (HttpsPort.HasValue && HttpsPort.Value > 0)
            {
                str.Append("vhost_https_port = ").Append(HttpsPort.Value).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(Token))
            {
                str.Append("token = ").Append(Token).AppendLine();
            }
            return str.ToString();
        }
    }
}