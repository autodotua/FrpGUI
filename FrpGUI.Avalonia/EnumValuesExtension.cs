using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace FrpGUI.Avalonia
{
    public class EnumValuesExtension : MarkupExtension
    {
        public EnumValuesExtension()
        {
        }

        public EnumValuesExtension(Type enumType)
        {
            EnumType = enumType;
        }

        [ConstructorArgument("enumType")]
        public Type EnumType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null)
                throw new ArgumentException("枚举类型不存在");
            return Enum.GetValues(EnumType);
        }
    }
}