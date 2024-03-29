﻿using FzLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrpGUI.Config
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

        public override char Type { get; } = 'c';

        public override object Clone()
        {
            var newItem = base.Clone() as ClientConfig;
            newItem.rules = Rules.Select(p => p.Clone() as Rule).ToList();
            return newItem;
        }

        public override string ToToml()
        {
            StringBuilder str = new StringBuilder();
            str.Append("serverAddr = ").Append('"').Append(ServerAddress).Append('"').AppendLine();
            str.Append("serverPort = ").Append(ServerPort).AppendLine();
            str.Append("loginFailExit = ").Append(LoginFailExit.ToString().ToLower()).AppendLine();

            str.Append("webServer.addr = ").Append('"').Append(AdminAddress).Append('"').AppendLine();
            str.Append("webServer.port = ").Append(adminPort).AppendLine();
            str.Append("webServer.user = ").Append('"').Append(adminUsername).Append('"').AppendLine();
            str.Append("webServer.password  = ").Append('"').Append(AdminPassword).Append('"').AppendLine();
            if (!string.IsNullOrWhiteSpace(Token))
            {
                str.Append("auth.token = ").Append('"').Append(Token).Append('"').AppendLine();
            }

            str.Append("transport.tls.enable = ").Append(EnableTls.ToString().ToLower()).AppendLine();
            str.Append("transport.poolCount = ").Append(PoolCount).AppendLine();

            str.AppendLine();
            foreach (var rule in Rules.Where(p => p.Enable && !string.IsNullOrEmpty(p.Name)))
            {
                str.Append(rule.ToToml()).AppendLine();
            }
            str.AppendLine();
            return str.ToString();
        }

        public override string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[common]").AppendLine();
            str.Append("server-addr = ").Append(ServerAddress).AppendLine();
            str.Append("server-port = ").Append(ServerPort).AppendLine();
            str.Append("pool-count = ").Append(PoolCount).AppendLine();
            str.Append("login-fail-exit = ").Append(LoginFailExit.ToString().ToLower()).AppendLine();
            str.Append("admin-addr = ").Append(AdminAddress).AppendLine();
            str.Append("admin-port = ").Append(AdminPort).AppendLine();
            str.Append("admin-user = ").Append(AdminUsername).AppendLine();
            str.Append("admin-pwd = ").Append(AdminPassword).AppendLine();
            if (!string.IsNullOrWhiteSpace(Token))
            {
                str.Append("token = ").Append(Token).AppendLine();
            }
            if (EnableTls)
            {
                str.Append("tls-enable = true").AppendLine();
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