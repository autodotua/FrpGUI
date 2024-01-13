using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Config;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        FrpConfigs.Add(new ServerConfig());
        FrpConfigs.Add(new ClientConfig());
        (FrpConfigs[^1] as ClientConfig).Rules.Add(new Rule()); 
        (FrpConfigs[^1] as ClientConfig).Rules.Add(new Rule()); 
    }
    private ServerPanel serverPanel = new ServerPanel();
    private ClientPanel clientPanel = new ClientPanel();

    [ObservableProperty]
    private ObservableCollection<FrpConfigBase> frpConfigs=new ObservableCollection<FrpConfigBase>();

    [ObservableProperty]
    private FrpConfigBase currentFrpConfig;

    [ObservableProperty]
    private UserControl currentPanel;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if(e.PropertyName==nameof(CurrentFrpConfig))
        {
            if(CurrentFrpConfig is ServerConfig)
            {
                serverPanel.LoadConfig(CurrentFrpConfig);
                CurrentPanel = serverPanel;
            }
            else if(CurrentFrpConfig is ClientConfig)
            {
                clientPanel.LoadConfig(CurrentFrpConfig);
                CurrentPanel= clientPanel;
            }
        }
    }
}
