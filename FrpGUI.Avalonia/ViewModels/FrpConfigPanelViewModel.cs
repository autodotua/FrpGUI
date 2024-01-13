using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Config;
using System.Collections.ObjectModel;

namespace FrpGUI.Avalonia.ViewModels;

public partial class FrpConfigPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    public FrpConfigBase frpConfig;

    [ObservableProperty]
    public ObservableCollection<Rule> rules;
}
