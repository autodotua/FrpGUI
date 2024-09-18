using Avalonia;
using Avalonia.Controls;

namespace FrpGUI.Avalonia.Views;

public partial class ProgressRingOverlay : UserControl
{
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<ProgressRingOverlay, bool>(nameof(IsActive));

    public ProgressRingOverlay()
    {
        InitializeComponent();
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }
}