using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Configs;
using FzLib.Avalonia;
using System.Diagnostics;

namespace FrpGUI.Avalonia.Views;

public partial class ClientPanel : ConfigPanelBase
{
    public ClientPanel()
    {
        InitializeComponent();
    }

    private void PanelBase_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Resources["RuleWidth"] = lstRules.Bounds.Width switch
        {
            < 840 => lstRules.Bounds.Width - 0,
            _ => 420d
        };
    }
}