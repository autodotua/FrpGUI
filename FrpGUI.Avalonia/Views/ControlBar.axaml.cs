using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrpGUI.Avalonia.Views;

public partial class ControlBar : UserControl
{
    //private MainViewModel viewModel;

    public ControlBar()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (TopLevel.GetTopLevel(this) is Window win)
        {
            PointerPressed += (s, e) =>
            {
                win.BeginMoveDrag(e);
            };
        }
    }
}