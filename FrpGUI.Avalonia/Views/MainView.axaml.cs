﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using System.Diagnostics;

namespace FrpGUI.Avalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        DataContext = new MainViewModel();
        InitializeComponent();
    }

    private async void AddRuleButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.Assert(mainPanel.Content is ClientPanel );
        (mainPanel.Content as ClientPanel).AddRule();
    }
}
