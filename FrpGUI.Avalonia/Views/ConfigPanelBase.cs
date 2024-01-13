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
            var vm = DataContext as FrpConfigPanelViewModel;
            vm.FrpConfig = frpConfig;
            if (frpConfig is ClientConfig cc)
            {
                vm.Rules = new System.Collections.ObjectModel.ObservableCollection<Rule>(cc.Rules);
            }
        }
    }
}
