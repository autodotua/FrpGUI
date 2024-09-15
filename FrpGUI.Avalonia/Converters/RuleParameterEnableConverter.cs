using Avalonia.Data.Converters;
using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using System;
using System.Globalization;

namespace FrpGUI.Avalonia.Converters
{
    public class RuleParameterEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NetType type = (NetType)value;
            return (parameter as string) switch
            {
                nameof(Rule.Domains) => type is NetType.HTTP or NetType.HTTPS,
                nameof(Rule.StcpKey) => type is NetType.STCP or NetType.STCP_Visitor,
                nameof(Rule.StcpServerName) => type is NetType.STCP_Visitor,
                nameof(Rule.RemotePort) => type is NetType.TCP or NetType.UDP,
                _ => throw new ArgumentException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}