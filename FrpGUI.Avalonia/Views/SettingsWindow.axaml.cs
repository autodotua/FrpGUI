using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FrpGUI.Avalonia.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        DataContext = new SettingWindowViewModel();
        InitializeComponent();
    }
}
