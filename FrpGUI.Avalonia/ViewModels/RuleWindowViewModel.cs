using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Config;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace FrpGUI.Avalonia.ViewModels;

public partial class RuleWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public Rule rule = new Rule();
    [ObservableProperty]
    public string errorMessage;

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
            if (range.Length != 2)
            {
                T("范围数量错误");
            }
            if (ushort.TryParse(range[0], out ushort from))
            {
                if (ushort.TryParse(range[1], out ushort to))
                {
                    if (from >= to)
                    {
                        T("范围起始大于结束");
                    }
                    for (ushort i = from; i <= to; i++)
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
                    T("端口号重复：" + p);
                }
            }
        }
    }
    private void T(string message)
    {
        throw new ArgumentException(message);
    }
    public bool Check()
    {
        try
        {
            if (Rule.Name.Length == 0) T("名称为空");
            if (Rule.Name.Length > 10) T("名称长度不可超过10");
            if (Rule.LocalPort.Length == 0) T("本地端口为空");
            if (Rule.LocalAddress.Length == 0) T("本地地址为空");

            ushort[] localPort = null;
            ushort[] remotePort = null;
            switch (Rule.Type)
            {
                case NetType.TCP:
                case NetType.UDP:
                    if (Rule.RemotePort.Length == 0) T("远程端口为空");
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
                    break;
            }
            if (Rule.Type is NetType.STCP or NetType.STCP_Visitor && string.IsNullOrWhiteSpace(Rule.STCPKey)) T("STCP密钥为空");
            if (Rule.Type is NetType.STCP_Visitor && string.IsNullOrWhiteSpace(Rule.STCPServerName)) T("STCP服务名为空");

            //暂不考虑端口不对应

            return true;
        }
        catch(Exception ex)
        {
            ErrorMessage = ex.Message;
            return false;
        }
    }
}
