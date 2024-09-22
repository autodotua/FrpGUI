using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Enums;
using System.Text;
using System.Text.Json.Serialization;

namespace FrpGUI.Models
{
    public partial class Rule : ObservableObject, IToFrpConfig, ICloneable
    {
        [ObservableProperty]
        private int bandwidthLimitKB = 1024;

        [ObservableProperty]
        private bool enableBandwidthLimit;

        [ObservableProperty]
        private bool compression;

        [ObservableProperty]
        private string domains;

        [ObservableProperty]
        private bool enable = true;

        [ObservableProperty]
        private bool encryption;

        [ObservableProperty]
        private string localAddress = "localhost";

        [ObservableProperty]
        private string localPort = "";

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private string remotePort = "";

        [ObservableProperty]
        [property: JsonPropertyName("STCPKey")]
        private string stcpKey;

        [ObservableProperty]
        [property: JsonPropertyName("STCPServerName")]
        private string stcpServerName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Domains), nameof(StcpKey), nameof(StcpServerName))]
        private NetType type = NetType.TCP;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public string ToToml()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine(Type == NetType.STCP_Visitor ? "[[visitors]]" : "[[proxies]]");
            str.Append("name = ").Append('"').Append(Name).Append('"').AppendLine();
            if (Type is NetType.STCP_Visitor)
            {
                str.Append("type = \"stcp\"").AppendLine();
            }
            else
            {
                str.Append("type = ").Append('"').Append(Type.ToString().ToLower()).Append('"').AppendLine();
            }
            if (Encryption)
            {
                str.AppendLine("transport.useEncryption = true");
            }
            if (Compression)
            {
                str.AppendLine("transport.useCompression = true");
            }
            if (EnableBandwidthLimit && BandwidthLimitKB > 0)
            {
                str.Append("transport.bandwidthLimit = \"").Append(BandwidthLimitKB).Append("KB\"").AppendLine();
            }

            switch (Type)
            {
                case NetType.HTTP or NetType.HTTPS:
                    str.Append("customDomains  = [").Append('"').Append(Domains).Append('"').Append(']').AppendLine();
                    break;

                case NetType.TCP or NetType.UDP:
                    str.Append("remotePort = ").Append(RemotePort).AppendLine();
                    break;
            }

            if (Type == NetType.STCP || Type == NetType.STCP_Visitor)
            {
                str.Append("secretKey = ").Append('"').Append(StcpKey).Append('"').AppendLine();
            }

            if (Type == NetType.STCP_Visitor)
            {
                str.Append("serverName = ").Append('"').Append(StcpServerName).Append('"').AppendLine();
                str.Append("bindAddr  = ").Append('"').Append(LocalAddress).Append('"').AppendLine();
                str.Append("bindPort = ").Append(LocalPort).AppendLine();
            }
            else
            {
                str.Append("localIP = ").Append('"').Append(LocalAddress).Append('"').AppendLine();
                str.Append("localPort = ").Append(LocalPort).AppendLine();
            }

            return str.ToString();
        }

        partial void OnTypeChanged(NetType value)
        {
            switch (value)
            {
                case NetType.TCP or NetType.UDP:
                    StcpKey = null;
                    StcpServerName = null;
                    Domains = null;
                    break;

                case NetType.HTTP or NetType.HTTPS:
                    StcpKey = null;
                    StcpServerName = null;
                    RemotePort = null;
                    break;

                case NetType.STCP:
                    RemotePort = null;
                    Domains = null;
                    StcpServerName = null;
                    break;

                case NetType.STCP_Visitor:
                    RemotePort = null;
                    Domains = null;
                    break;
            }
        }
    }
}