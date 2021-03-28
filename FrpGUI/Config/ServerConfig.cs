using FzLib.Extension;
using System.ComponentModel;
using System.Text;

namespace FrpGUI
{
    public class ServerConfig : IToIni
    {
        private string token;

        public string Token
        {
            get => token;
            set => this.SetValueAndNotify(ref token, value, nameof(Token));
        }

        private ushort port = 7000;

        public ushort Port
        {
            get => port;
            set => this.SetValueAndNotify(ref port, value, nameof(Port));
        }

        private ushort dashBoardPort = 7500;

        public ushort DashBoardPort
        {
            get => dashBoardPort;
            set => this.SetValueAndNotify(ref dashBoardPort, value, nameof(DashBoardPort));
        }

        private string dashBoardUsername = "admin";

        public string DashBoardUsername
        {
            get => dashBoardUsername;
            set => this.SetValueAndNotify(ref dashBoardUsername, value, nameof(DashBoardUsername));
        }

        private string dashBoardPassword = "admin";

        public string DashBoardPassword
        {
            get => dashBoardPassword;
            set => this.SetValueAndNotify(ref dashBoardPassword, value, nameof(DashBoardPassword));
        }

        private ushort? httpPort;

        public ushort? HttpPort
        {
            get => httpPort;
            set => this.SetValueAndNotify(ref httpPort, value, nameof(HttpPort));
        }

        private ushort? httpsPort;

        public ushort? HttpsPort
        {
            get => httpsPort;
            set => this.SetValueAndNotify(ref httpsPort, value, nameof(HttpsPort));
        }

        public event PropertyChangedEventHandler PropertyChanged;

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