using FzLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrpGUI
{
    public class ClientConfig : FrpConfigBase
    {
        private string adminAddress = "localhost";
        private string adminPassword = "admin";
        private ushort adminPort = 7400;
        private string adminUsername = "admin";
        private bool enableTls;
        private bool loginFailExit = false;
        private short poolCount = 50;
        private List<Rule> rules = new List<Rule>();
        private string serverAddress;
        private ushort serverPort = 7000;
        private string token;

        public ClientConfig()
        {
            Name = "客户端1";
        }

        public string AdminAddress
        {
            get => adminAddress;
            set => this.SetValueAndNotify(ref adminAddress, value, nameof(AdminAddress));
        }
        public string AdminPassword
        {
            get => adminPassword;
            set => this.SetValueAndNotify(ref adminPassword, value, nameof(AdminPassword));
        }

        public ushort AdminPort
        {
            get => adminPort;
            set => this.SetValueAndNotify(ref adminPort, value, nameof(AdminPort));
        }

        public string AdminUsername
        {
            get => adminUsername;
            set => this.SetValueAndNotify(ref adminUsername, value, nameof(AdminUsername));
        }

        public bool EnableTls
        {
            get => enableTls;
            set => this.SetValueAndNotify(ref enableTls, value, nameof(EnableTls));
        }
        public bool LoginFailExit
        {
            get => loginFailExit;
            set => this.SetValueAndNotify(ref loginFailExit, value, nameof(LoginFailExit));
        }

        public short PoolCount
        {
            get => poolCount;
            set => this.SetValueAndNotify(ref poolCount, value, nameof(PoolCount));
        }

        public List<Rule> Rules
        {
            get => rules;
            set => this.SetValueAndNotify(ref rules, value, nameof(Rules));
        }

        public string ServerAddress
        {
            get => serverAddress;
            set => this.SetValueAndNotify(ref serverAddress, value, nameof(ServerAddress));
        }

        public ushort ServerPort
        {
            get => serverPort;
            set => this.SetValueAndNotify(ref serverPort, value, nameof(ServerPort));
        }

        public string Token
        {
            get => token;
            set => this.SetValueAndNotify(ref token, value, nameof(Token));
        }

        public override string Type { get; } = "c";

        public override object Clone()
        {
            var newItem = base.Clone() as ClientConfig;
            newItem.rules = Rules.Select(p => p.Clone() as Rule).ToList();
            return newItem;
        }

        public override string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[common]").AppendLine();
            str.Append("server_addr = ").Append(ServerAddress).AppendLine();
            str.Append("server_port = ").Append(ServerPort).AppendLine();
            str.Append("pool_count = ").Append(PoolCount).AppendLine();
            str.Append("login_fail_exit = ").Append(LoginFailExit.ToString().ToLower()).AppendLine();
            str.Append("admin_addr = ").Append(AdminAddress).AppendLine();
            str.Append("admin_port = ").Append(AdminPort).AppendLine();
            str.Append("admin_user = ").Append(AdminUsername).AppendLine();
            str.Append("admin_pwd = ").Append(AdminPassword).AppendLine();
            if (!string.IsNullOrWhiteSpace(Token))
            {
                str.Append("token = ").Append(Token).AppendLine();
            }
            if(EnableTls)
            {
                str.Append("tls_enable = true").AppendLine();
            }
            foreach (var rule in Rules.Where(p => p.Enable && !string.IsNullOrEmpty(p.Name)))
            {
                str.Append(rule.ToIni()).AppendLine();
            }
            str.AppendLine();
            return str.ToString();
        }
    }
}