using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Windows;

namespace FrpGUI.Avalonia.Converters
{
    /// <summary>
    /// 若值为非空或字符串不为空，则返回true/Visible，否则返回false/Collapse/Hidden。支持'i‘反转。
    /// </summary>
    public class NotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;
            if (value == null)
            {
                result = false;
            }
            if (value is string && string.IsNullOrEmpty(value as string))
            {
                result = false;
            }
            result = ConverterHelper.GetInverseResult(result, parameter);
            if (targetType == typeof(bool) || targetType == typeof(bool?))
            {
                return result;
            }
            throw new ArgumentException(nameof(targetType));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}