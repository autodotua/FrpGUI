using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using FzLib.Avalonia;
using FzLib.Avalonia.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class ControlBar : UserControl
{
    public ControlBar()
    {
        InitializeComponent();
    }
    private void AddRuleButton_Click(object sender, RoutedEventArgs e)
    {
        var mainView = GetMainView();
        Debug.Assert((mainView.DataContext as MainViewModel).CurrentPanel is ClientPanel);
        var clientPanel = (mainView.DataContext as MainViewModel).CurrentPanel as ClientPanel;
        clientPanel.AddRule();
    }

    private MainView GetMainView()
    {
        return this.GetVisualAncestors().OfType<MainView>().FirstOrDefault() ?? throw new System.Exception("’“≤ªµΩMainView");
    }

    private async void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        var mainView = GetMainView();
        try
        {
            await (mainView.DataContext as MainViewModel).CurrentFrpConfig.RestartAsync();
        }
        catch (Exception ex)
        {
            await this.GetWindow().ShowErrorDialogAsync("÷ÿ∆Ù ß∞‹", ex);
        }
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var mainView = GetMainView();
        try
        {
            (mainView.DataContext as MainViewModel).CurrentFrpConfig.Start();
        }
        catch (Exception ex)
        {
            await this.GetWindow().ShowErrorDialogAsync("∆Ù∂Ø ß∞‹", ex);
        }
    }

    private async void StopButton_Click(object sender, RoutedEventArgs e)
    {
        var mainView = GetMainView();
        try
        {
            await (mainView.DataContext as MainViewModel).CurrentFrpConfig.StopAsync();
        }
        catch (Exception ex)
        {
            await this.GetWindow().ShowErrorDialogAsync("Õ£÷π ß∞‹", ex);
        }
    }

}