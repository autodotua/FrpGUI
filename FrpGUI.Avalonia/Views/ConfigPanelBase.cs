using Avalonia.Controls;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views
{
    public class ConfigPanelBase : UserControl
    {
        public void LoadConfig(FrpConfigBase frpConfig)
        {
            (DataContext as FrpConfigPanelViewModel).FrpConfig= frpConfig;
        }
    }
}
