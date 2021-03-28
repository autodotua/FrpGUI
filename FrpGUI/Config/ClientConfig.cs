using FzLib.Extension;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrpGUI
{
    public class ClientConfig : IToIni
    {
        private string serverAddress;

        public string ServerAddress
        {
            get => serverAddress;
            set => this.SetValueAndNotify(ref serverAddress, value, nameof(ServerAddress));
        }

        private string token;

        public string Token
        {
            get => token;
            set => this.SetValueAndNotify(ref token, value, nameof(Token));
        }

        private ushort serverPort = 7000;

        public ushort ServerPort
        {
            get => serverPort;
            set => this.SetValueAndNotify(ref serverPort, value, nameof(ServerPort));
        }

        private bool loginFailExit = false;

        public bool LoginFailExit
        {
            get => loginFailExit;
            set => this.SetValueAndNotify(ref loginFailExit, value, nameof(LoginFailExit));
        }

        private string adminAddress = "localhost";

        public string AdminAddress
        {
            get => adminAddress;
            set => this.SetValueAndNotify(ref adminAddress, value, nameof(AdminAddress));
        }

        private ushort adminPort = 7400;

        public ushort AdminPort
        {
            get => adminPort;
            set => this.SetValueAndNotify(ref adminPort, value, nameof(AdminPort));
        }

        private string adminUsername = "admin";

        public string AdminUsername
        {
            get => adminUsername;
            set => this.SetValueAndNotify(ref adminUsername, value, nameof(AdminUsername));
        }

        private string adminPassword = "admin";

        public string AdminPassword
        {
            get => adminPassword;
            set => this.SetValueAndNotify(ref adminPassword, value, nameof(AdminPassword));
        }

        private List<Rule> rules = new List<Rule>();

        public List<Rule> Rules
        {
            get => rules;
            set => this.SetValueAndNotify(ref rules, value, nameof(Rules));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[common]").AppendLine();
            str.Append("server_addr = ").Append(ServerAddress).AppendLine();
            str.Append("server_port = ").Append(ServerPort).AppendLine();
            str.Append("login_fail_exit = ").Append(LoginFailExit.ToString().ToLower()).AppendLine();
            str.Append("admin_addr = ").Append(AdminAddress).AppendLine();
            str.Append("admin_port = ").Append(AdminPort).AppendLine();
            str.Append("admin_user = ").Append(AdminUsername).AppendLine();
            str.Append("admin_pwd = ").Append(AdminPassword).AppendLine();
            if (!string.IsNullOrWhiteSpace(Token))
            {
                str.Append("token = ").Append(Token).AppendLine();
            }
            foreach (var rule in Rules.Where(p => p.Enable && !string.IsNullOrEmpty(p.Name)))
            {
                str.Append(rule.ToIni()).AppendLine();
            }
            str.AppendLine();
            return str.ToString();
        }
    }

    public class Rule : IToIni
    {
        public bool Enable { get; set; } = true;
        private string name;

        public string Name
        {
            get => name;
            set => this.SetValueAndNotify(ref name, value, nameof(Name));
        }

        private NetType type = NetType.TCP;

        public NetType Type
        {
            get => type;
            set => this.SetValueAndNotify(ref type, value, nameof(Type), nameof(Domains), nameof(STCPKey), nameof(STCPServerName));
        }

        private string localAddress = "localhost";

        public string LocalAddress
        {
            get => localAddress;
            set => this.SetValueAndNotify(ref localAddress, value, nameof(LocalAddress));
        }

        private ushort localPort;

        public ushort LocalPort
        {
            get => localPort;
            set => this.SetValueAndNotify(ref localPort, value, nameof(LocalPort));
        }

        private ushort remotePort;

        public ushort RemotePort
        {
            get => remotePort;
            set => this.SetValueAndNotify(ref remotePort, value, nameof(RemotePort));
        }

        private string domains;

        public string Domains
        {
            get => domains;
            set => this.SetValueAndNotify(ref domains, value, nameof(Domains));
        }

        private bool encryption;

        public bool Encryption
        {
            get => encryption;
            set => this.SetValueAndNotify(ref encryption, value, nameof(Encryption));
        }

        private bool compression;

        public bool Compression
        {
            get => compression;
            set => this.SetValueAndNotify(ref compression, value, nameof(Compression));
        }

        private string stcpKey;

        public string STCPKey
        {
            get => stcpKey;
            set => this.SetValueAndNotify(ref stcpKey, value, nameof(STCPKey));
        }

        private string stcpServerName;

        public string STCPServerName
        {
            get => stcpServerName;
            set => this.SetValueAndNotify(ref stcpServerName, value, nameof(STCPServerName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[").Append(Name).Append("]").AppendLine();
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

    public enum NetType
    {
        TCP,
        UDP,
        HTTP,
        HTTPS,
        STCP,

        [Description("STCP访问者")]
        STCP_Visitor,

        //XTCP
    }
}