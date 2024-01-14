using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using MsBox.Avalonia;
using MsBox.Avalonia.Models;
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
            await ShowErrorAsync("÷ÿ∆Ù ß∞‹", ex.Message);
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
            await ShowErrorAsync("∆Ù∂Ø ß∞‹", ex.Message);
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
            await ShowErrorAsync("Õ£÷π ß∞‹", ex.Message);
        }
    }

    private async Task ShowErrorAsync(string title, string message)
    {
        await MessageBoxManager.GetMessageBoxCustom(new MsBox.Avalonia.Dto.MessageBoxCustomParams()
        {
            ContentTitle = title,
            ContentMessage = message,
            FontFamily = this.FontFamily,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ButtonDefinitions = new List<ButtonDefinition>
                {
                    new ButtonDefinition { Name = "»∑∂®", },
                },
        }).ShowAsync();
    }
}