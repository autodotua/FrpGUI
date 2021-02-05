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
        public short ServerPort { get; set; } = 7000;
        public List<Rule> Rules { get; set; } = new List<Rule>();

        public string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[common]").AppendLine();
            str.Append("server_addr = ").Append(ServerAddress).AppendLine();
            str.Append("server_port = ").Append(ServerPort).AppendLine();
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
        public short LocalPort { get; set; }

        [Display(Name = "服务器端口")]
        public short RemotePort { get; set; }

        public string ToIni()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[").Append(Name).Append("]").AppendLine();
            str.Append("type = ").Append(Type.ToString().ToLower()).AppendLine();
            str.Append("local_ip = ").Append(LocalAddress).AppendLine();
            str.Append("local_port = ").Append(LocalPort).AppendLine();
            str.Append("remote_port = ").Append(RemotePort).AppendLine();
            return str.ToString();
        }
    }
}