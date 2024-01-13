using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Config;

namespace FrpGUI.Avalonia.ViewModels;

public partial class FrpConfigPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    public FrpConfigBase frpConfig;
}
