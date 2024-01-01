using FzLib;
using System.Text;

namespace FrpGUI.Config
{
    public class ServerConfig : FrpConfigBase
    {
        private string dashBoardPassword = "admin";
        private ushort dashBoardPort = 7500;
        private string dashBoardUsername = "admin";
        private ushort? httpPort;
        private ushort? httpsPort;
        private short maxPoolCount = 100;
        private ushort port = 7000;
        private bool tlsOnly;
        private string token;

        public ServerConfig()
        {
            Name = "服务端";
        }

        public string DashBoardPassword
        {
            get => dashBoardPassword;
            set => this.SetValueAndNotify(ref dashBoardPassword, value, nameof(DashBoardPassword));
        }

        public ushort DashBoardPort
        {
            get => dashBoardPort;
            set => this.SetValueAndNotify(ref dashBoardPort, value, nameof(DashBoardPort));
        }

        public string DashBoardUsername
        {
            get => dashBoardUsername;
            set => this.SetValueAndNotify(ref dashBoardUsername, value, nameof(DashBoardUsername));
        }
        public ushort? HttpPort
        {
            get => httpPort;
            set => this.SetValueAndNotify(ref httpPort, value, nameof(HttpPort));
        }

        public ushort? HttpsPort
        {
            get => httpsPort;
            set => this.SetValueAndNotify(ref httpsPort, value, nameof(HttpsPort));
        }

        public short MaxPoolCount
        {
            get => maxPoolCount;
            set => this.SetValueAndNotify(ref maxPoolCount, value, nameof(MaxPoolCount));
        }

        public ushort Port
        {
            get => port;
            set => this.SetValueAndNotify(ref port, value, nameof(Port));
        }

        public bool TlsOnly
        {
            get => tlsOnly;
            set => this.SetValueAndNotify(ref tlsOnly, value, nameof(TlsOnly));
        }
        public string Token
        {
            get => token;
            set => this.SetValueAndNotify(ref token, value, nameof(Token));
        }

        public override char Type { get; } = 's';

        public override string ToToml()
        {
            StringBuilder str = new StringBuilder();
            str.Append("bindPort = ").Append(Port).AppendLine();
            str.Append("transport.maxPoolCount = ").Append(MaxPoolCount).AppendLine();
            str.Append("webServer.port = ").Append(DashBoardPort).AppendLine();
            str.Append("webServer.user = ").Append('"').Append(DashBoardUsername).Append('"').AppendLine();
            str.Append("webServer.password  = ").Append('"').Append(DashBoardPassword).Append('"').AppendLine();

            if (HttpPort.HasValue && HttpPort.Value > 0)
            {
                str.Append("vhostHTTPPort = ").Append(HttpPort.Value).AppendLine();
            }
            if (HttpsPort.HasValue && HttpsPort.Value > 0)
            {
                str.Append("vhostHTTPSPort = ").Append(HttpsPort.Value).AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(Token))
            {
                str.Append("auth.token = ").Append('"').Append(Token).Append('"').AppendLine();
            }

            str.Append("transport.tls.enable = ").Append(TlsOnly.ToString().ToLower()).AppendLine();

            return str.ToString();
        }

        public override string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[common]").AppendLine();
            str.Append("bind_port = ").Append(Port).AppendLine();
            str.Append("max_pool_count = ").Append(MaxPoolCount).AppendLine();
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
            if (TlsOnly)
            {
                str.Append("tls_only = true").AppendLine();
            }
            return str.ToString();
        }
    }
}