using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;

namespace FrpGUI.Avalonia;

public partial class ClientPanel : ConfigPanelBase
{
    public ClientPanel()
    {
        DataContext = new FrpConfigPanelViewModel();
        InitializeComponent();
    }
}