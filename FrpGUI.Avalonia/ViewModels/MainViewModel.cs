using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Avalonia.Views;
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
        FrpConfigs = new ObservableCollection<FrpConfigBase>(AppConfig.Instance.FrpConfigs);
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
