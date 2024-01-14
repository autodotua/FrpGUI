using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using System.Diagnostics;

namespace FrpGUI.Avalonia;

public partial class ClientPanel : ConfigPanelBase
{
    public ClientPanel()
    {
        DataContext = new FrpConfigPanelViewModel();
        InitializeComponent();
    }

    public async void AddRule()
    {
        var dialog = new RuleWindow();
        var rule = await dialog.ShowDialog<Rule>(this.GetVisualRoot() as Window);
        (DataContext as FrpConfigPanelViewModel).Rules.Add(rule);
    }
}