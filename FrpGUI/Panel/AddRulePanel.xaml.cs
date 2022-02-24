using ModernWpf.FzExtension.CommonDialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrpGUI
{
    /// <summary>
    /// ServerPanel.xaml 的交互逻辑
    /// </summary>
    public partial class AddRulePanel : UserControl
    {
        public AddRulePanel(IEnumerable<Rule> rules, Rule rule = null)
        {
            Rule = (rule?.Clone() as Rule) ?? new Rule();
            InitializeComponent();
            DataContext = this;
            Rules = rules.ToArray();
        }
        public Rule Rule { get; private set; }
        public bool Save { get; private set; }
        public Rule[] Rules { get; }

        public event EventHandler RequestClosing;

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (await CheckAsync())
            {
                Save = true;
                RequestClosing?.Invoke(this, e);
            }

        }
        private void T(string message)
        {
            throw new FormatException(message);
        }
        private ushort[] GetPorts(string port)
        {
            if (ushort.TryParse(port, out ushort result))
            {
                return new ushort[] { result };
            }
            HashSet<ushort> ports = new HashSet<ushort>();
            foreach (var part in port.Split(','))
            {
                if (ushort.TryParse(part, out ushort r))
                {
                    Add(r);
                }
                var range = part.Split('-');
                if(range.Length!=2)
                {
                    T("范围数量错误");
                }
                if (ushort.TryParse(range[0], out ushort from))
                {
                    if(ushort.TryParse(range[1], out ushort to))
                    {
                        if(from>=to)
                        {
                            T("范围起始大于结束");
                        }
                        for(ushort i = from; i <= to; i++)
                        {
                            Add(i);
                        }
                    }
                    else
                    {
                        T("范围解析错误");
                    }
                }
                else
                {
                    T("范围解析错误");
                }

            }
            return ports.ToArray();
            void Add(ushort p)
            {
                if (ports.Contains(p))
                {
                    if (!ports.Add(p))
                    {
                        T("端口号重复："+p);
                    }
                }
            }
        }
        private async Task<bool> CheckAsync()
        {
            try
            {
                if (Rule.Name.Length == 0) T("名称为空");
                if (Rule.Name.Length > 10) T("名称长度不可超过10");
                if (Rule.LocalPort.Length == 0) T("本地端口为空");
                if (Rule.RemotePort.Length == 0) T("远程端口为空");
                if (Rule.LocalAddress.Length == 0) T("本地地址为空");
                ushort[] localPort=null;
                ushort[] remotePort=null;
                switch (Rule.Type)
                {
                    case NetType.TCP:
                    case NetType.UDP:
                        try
                        {
                            localPort = GetPorts(Rule.LocalPort);
                        }
                        catch (FormatException ex)
                        {
                            T("本地端口" + ex.Message);
                        }
                        try
                        {
                            remotePort = GetPorts(Rule.RemotePort);
                        }
                        catch (FormatException ex)
                        {
                            T("远程端口" + ex.Message);
                        }
                        if (localPort.Length != remotePort.Length)
                        {
                            T("本地端口和远程端口数量不同");
                        }
                        break;
                    case NetType.HTTP:
                    case NetType.HTTPS:
                    case NetType.STCP:
                    case NetType.STCP_Visitor:
                        if(!ushort.TryParse(Rule.LocalAddress, out _)) T("本地端口有误");                 
                        break;
                }
                if (Rule.Type is NetType.STCP or NetType.STCP_Visitor && string.IsNullOrWhiteSpace(Rule.STCPKey)) T("STCP密钥为空");
                if (Rule.Type is NetType.STCP_Visitor && string.IsNullOrWhiteSpace(Rule.STCPServerName)) T("STCP服务名为空");
             
                //暂不考虑端口不对应


                return true;
            }
            catch (FormatException ex)
            {
                await CommonDialog.ShowErrorDialogAsync(ex.Message);
                return false;
            }


        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RequestClosing?.Invoke(this, e);
        }
    }

}