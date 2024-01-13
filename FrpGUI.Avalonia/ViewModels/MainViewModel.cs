using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Config;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        FrpConfigs.Add(new ServerConfig());
        FrpConfigs.Add(new ClientConfig());
    }
    [ObservableProperty]
    private ObservableCollection<FrpConfigBase> frpConfigs=new ObservableCollection<FrpConfigBase>();
}
