using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Configs;
using FzLib.Avalonia;
using FzLib.Avalonia.Dialogs;
using FzLib.Avalonia.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
