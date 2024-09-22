using Avalonia.Controls;

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