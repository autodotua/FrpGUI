using System.Text;

namespace FrpGUI
{
    public class ServerConfig : IToIni
    {
        public short Port { get; set; } = 7000;
        public short DashBoardPort { get; set; } = 7500;
        public string DashBoardUsername { get; set; } = "admin";
        public string DashBoardPassword { get; set; } = "admin";

        public string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[common]").AppendLine();
            str.Append("bind_port = ").Append(Port).AppendLine();
            str.Append("dashboard_port = ").Append(DashBoardPort).AppendLine();
            str.Append("dashboard_user = ").Append(DashBoardUsername).AppendLine();
            str.Append("dashboard_pwd = ").Append(DashBoardPassword).AppendLine();
            return str.ToString();
        }
    }
}