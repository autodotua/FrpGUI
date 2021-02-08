using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrpGUI
{
    public class ClientConfig : IToIni
    {
        public string ServerAddress { get; set; }
        public string Token { get; set; }
        public ushort ServerPort { get; set; } = 7000;
        public bool LoginFailExit { get; set; } = false;
        public string AdminAddress { get; set; } = "localhost";
        public ushort AdminPort { get; set; } = 7400;
        public string AdminUsername { get; set; } = "admin";
        public string AdminPassword { get; set; } = "admin";
        public List<Rule> Rules { get; set; } = new List<Rule>();

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
            str.AppendLine();
            if (!string.IsNullOrWhiteSpace(Token))
            {
                str.Append("token = ").Append(Token).AppendLine();
            }
            foreach (var rule in Rules.Where(p => !string.IsNullOrEmpty(p.Name)))
            {
                str.Append(rule.ToIni()).AppendLine();
            }
            return str.ToString();
        }
    }

    public class Rule : IToIni
    {
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "类型")]
        public NetType Type { get; set; } = NetType.TCP;

        [Display(Name = "本地IP")]
        public string LocalAddress { get; set; } = "localhost";

        [Display(Name = "本地端口")]
        public ushort LocalPort { get; set; }

        [Display(Name = "远程端口")]
        public ushort RemotePort { get; set; }

        [Display(Name = "域名")]
        public string Domains { get; set; }

        public string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[").Append(Name).Append("]").AppendLine();
            str.Append("type = ").Append(Type.ToString().ToLower()).AppendLine();
            str.Append("local_ip = ").Append(LocalAddress).AppendLine();
            str.Append("local_port = ").Append(LocalPort).AppendLine();
            if (Type != NetType.HTTP && Type != NetType.HTTPS)
            {
                str.Append("remote_port = ").Append(RemotePort).AppendLine();
            }
            if (Type == NetType.HTTP || Type == NetType.HTTPS)
            {
                str.Append("custom_domains = ").Append(Domains).AppendLine();
            }
            return str.ToString();
        }
    }

    public enum NetType
    {
        TCP,
        UDP,
        HTTP,
        HTTPS,
        //STCP,
        //XTCP
    }
}