using Avalonia.Data.Converters;
using Avalonia.Media;
using FrpGUI.Enums;
using System;
using System.Globalization;

namespace FrpGUI.Avalonia.Converters
{
    public class ProcessStatus2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            ProcessStatus status = (ProcessStatus)value;
            if (status == ProcessStatus.Stopped)
            {
                return Brushes.Red;
            }
            return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}