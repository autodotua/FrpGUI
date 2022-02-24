using FzLib;
using System;
using System.ComponentModel;
using System.Text;

namespace FrpGUI
{
    public class Rule : IToIni, ICloneable
    {
        private bool compression;
        private string domains;
        private bool enable = true;
        private bool encryption;
        private string localAddress = "localhost";
        private string localPort = "";
        private string name = "";
        private string remotePort="";
        private string stcpKey;
        private string stcpServerName;
        private NetType type = NetType.TCP;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Compression
        {
            get => compression;
            set => this.SetValueAndNotify(ref compression, value, nameof(Compression));
        }

        public string Domains
        {
            get => domains;
            set => this.SetValueAndNotify(ref domains, value, nameof(Domains));
        }

        public bool Enable
        {
            get => enable;
            set => this.SetValueAndNotify(ref enable, value, nameof(Enable));
        }


        public bool Encryption
        {
            get => encryption;
            set => this.SetValueAndNotify(ref encryption, value, nameof(Encryption));
        }

        public string LocalAddress
        {
            get => localAddress;
            set => this.SetValueAndNotify(ref localAddress, value, nameof(LocalAddress));
        }

        public string LocalPort
        {
            get => localPort;
            set => this.SetValueAndNotify(ref localPort, value, nameof(LocalPort));
        }

        public string Name
        {
            get => name;
            set => this.SetValueAndNotify(ref name, value, nameof(Name));
        }

        public string RemotePort
        {
            get => remotePort;
            set => this.SetValueAndNotify(ref remotePort, value, nameof(RemotePort));
        }

        public string STCPKey
        {
            get => stcpKey;
            set => this.SetValueAndNotify(ref stcpKey, value, nameof(STCPKey));
        }

        public string STCPServerName
        {
            get => stcpServerName;
            set => this.SetValueAndNotify(ref stcpServerName, value, nameof(STCPServerName));
        }

        public NetType Type
        {
            get => type;
            set => this.SetValueAndNotify(ref type, value, nameof(Type), nameof(Domains), nameof(STCPKey), nameof(STCPServerName));
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append('[')
                .Append(localPort.Contains(',')||localPort.Contains('-')?"range:":"")
                .Append(Name)
                .Append(']')
                .AppendLine();
            str.Append("type = ").Append(Type.ToString().Split('_')[0].ToLower()).AppendLine();
            str.Append("use_encryption = ").Append(Encryption.ToString().ToLower()).AppendLine();
            str.Append("use_compression = ").Append(Compression.ToString().ToLower()).AppendLine();
            if (Type == NetType.HTTP || Type == NetType.HTTPS)
            {
                str.Append("custom_domains = ").Append(Domains).AppendLine();
            }
            else
            {
                str.Append("remote_port = ").Append(RemotePort).AppendLine();
            }
            if (Type == NetType.STCP || Type == NetType.STCP_Visitor)
            {
                str.Append("sk = ").Append(STCPKey).AppendLine();
            }

            if (Type == NetType.STCP_Visitor)
            {
                str.Append("role = visitor").AppendLine();
                str.Append("server_name = ").Append(STCPServerName).AppendLine();
                str.Append("bind_addr = ").Append(LocalAddress).AppendLine();
                str.Append("bind_port = ").Append(LocalPort).AppendLine();
            }
            else
            {
                str.Append("local_ip = ").Append(LocalAddress).AppendLine();
                str.Append("local_port = ").Append(LocalPort).AppendLine();
            }

            str.AppendLine();
            return str.ToString();
        }
    }
}