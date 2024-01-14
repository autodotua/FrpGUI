using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;

namespace FrpGUI.Avalonia.Views;

public partial class ServerPanel : ConfigPanelBase
{
    public ServerPanel()
    {
        DataContext = new FrpConfigPanelViewModel();
        InitializeComponent();
    }
}