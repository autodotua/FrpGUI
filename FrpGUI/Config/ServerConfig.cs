﻿using CommunityToolkit.Mvvm.ComponentModel;
using FzLib;
using System.Text;

namespace FrpGUI.Config
{
    public partial class ServerConfig : FrpConfigBase
    {
        [ObservableProperty]
        private string dashBoardPassword = "admin";

        [ObservableProperty]
        private ushort dashBoardPort = 7500;

        [ObservableProperty]
        private string dashBoardUsername = "admin";

        [ObservableProperty]
        private ushort? httpPort;

        [ObservableProperty]
        private ushort? httpsPort;

        [ObservableProperty]
        private short maxPoolCount = 100;

        [ObservableProperty]
        private ushort port = 7000;

        [ObservableProperty]
        private bool tlsOnly;

        [ObservableProperty]
        private string token;

        public ServerConfig()
        {
            Name = "服务端";
        }

        public override char Type { get; } = 's';

        public override string ToToml()
        {
            StringBuilder str = new StringBuilder();
            str.Append("bindPort = ").Append(Port).AppendLine();
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

            str.Append("transport.tls.force = ").Append(TlsOnly.ToString().ToLower()).AppendLine();
            str.Append("transport.maxPoolCount = ").Append(MaxPoolCount).AppendLine();

            return str.ToString();
        }
    }
}