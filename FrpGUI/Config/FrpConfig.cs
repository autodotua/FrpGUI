using FzLib.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FrpGUI
{
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

    public class ClientConfig : FrpConfigBase
    {
        private string adminAddress = "localhost";
        private string adminPassword = "admin";
        private ushort adminPort = 7400;
        private string adminUsername = "admin";
        private bool loginFailExit = false;
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

        public bool LoginFailExit
        {
            get => loginFailExit;
            set => this.SetValueAndNotify(ref loginFailExit, value, nameof(LoginFailExit));
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

        public override string ToIni()
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

        public override object Clone()
        {
            var newItem = base.Clone() as ClientConfig;
            newItem.rules = Rules.Select(p => p.Clone() as Rule).ToList();
            return newItem;
        }
    }

    public abstract class FrpConfigBase : IToIni, ICloneable
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        private bool autoStart;

        private string name;

        private ProcessStatus processStatus = ProcessStatus.NotRun;

        public FrpConfigBase()
        {
            Process.Exited += Process_Exited;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler StatusChanged;

        public bool AutoStart
        {
            get => autoStart;
            set => this.SetValueAndNotify(ref autoStart, value, nameof(AutoStart));
        }

        public string Name
        {
            get => name;
            set => this.SetValueAndNotify(ref name, value, nameof(Name));
        }

        [JsonIgnore]
        public ProcessHelper Process { get; protected set; } = new ProcessHelper();

        [JsonIgnore]
        public ProcessStatus ProcessStatus
        {
            get => processStatus;
            private set => this.SetValueAndNotify(ref processStatus, value, nameof(ProcessStatus));
        }

        public abstract string Type { get; }

        public void ChangeStatus(ProcessStatus status)
        {
            Debug.WriteLine("进程状态改变：" + status.ToString());
            ProcessStatus = status;
            StatusChanged?.Invoke(this, new EventArgs());
        }

        public async Task RestartAsync()
        {
            ChangeStatus(ProcessStatus.Busy);
            await Process.RestartAsync();
            ChangeStatus(ProcessStatus.Running);
            Config.Instance.Save();
        }

        public void Start()
        {
            ChangeStatus(ProcessStatus.Busy);
            Process.Start(Type, this);
            ChangeStatus(ProcessStatus.Running);
            Config.Instance.Save();
        }

        public async Task StopAsync()
        {
            ChangeStatus(ProcessStatus.Busy);
            await Process.StopAsync();
            ChangeStatus(ProcessStatus.NotRun);
        }

        public abstract string ToIni();

        private void Process_Exited(object sender, EventArgs e)
        {
            ChangeStatus(ProcessStatus.NotRun);
        }

        public virtual object Clone()
        {
            var newItem = MemberwiseClone() as FrpConfigBase;
            newItem.processStatus = ProcessStatus.NotRun;
            newItem.Process = new ProcessHelper();
            return newItem;
        }
    }

    public class Rule : IToIni, ICloneable
    {
        private bool compression;
        private string domains;
        private bool encryption;
        private string localAddress = "localhost";
        private ushort localPort;
        private string name;
        private ushort remotePort;
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

        public bool Enable { get; set; } = true;

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

        public ushort LocalPort
        {
            get => localPort;
            set => this.SetValueAndNotify(ref localPort, value, nameof(LocalPort));
        }

        public string Name
        {
            get => name;
            set => this.SetValueAndNotify(ref name, value, nameof(Name));
        }

        public ushort RemotePort
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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class ServerConfig : FrpConfigBase
    {
        private string dashBoardPassword = "admin";
        private ushort dashBoardPort = 7500;
        private string dashBoardUsername = "admin";
        private ushort? httpPort;
        private ushort? httpsPort;
        private ushort port = 7000;
        private string token;

        public ServerConfig()
        {
            Name = "服务端1";
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

        public ushort Port
        {
            get => port;
            set => this.SetValueAndNotify(ref port, value, nameof(Port));
        }

        public string Token
        {
            get => token;
            set => this.SetValueAndNotify(ref token, value, nameof(Token));
        }

        public override string Type { get; } = "s";

        public override string ToIni()
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