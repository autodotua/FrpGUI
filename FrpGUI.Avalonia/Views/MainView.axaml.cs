using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using FzLib.Avalonia;
using FzLib.Avalonia.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        DataContext = new MainViewModel();
        InitializeComponent();
    }

    private void AddClientButton_Click(object sender, RoutedEventArgs e)
    {
        var newConfig = new ClientConfig();
        (DataContext as MainViewModel).FrpConfigs.Add(newConfig);
        (DataContext as MainViewModel).CurrentFrpConfig = newConfig;
    }

    private void AddServerButton_Click(object sender, RoutedEventArgs e)
    {
        var newConfig = new ServerConfig();
        (DataContext as MainViewModel).FrpConfigs.Add(newConfig);
        (DataContext as MainViewModel).CurrentFrpConfig = newConfig;
    }
    private void CreateCopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var config = (sender as MenuItem).DataContext as FrpConfigBase;
        var newConfig = config.Clone() as FrpConfigBase;
        newConfig.Name = newConfig.Name + "（复制）";
        (DataContext as MainViewModel).FrpConfigs.Add(newConfig);
        (DataContext as MainViewModel).CurrentFrpConfig = newConfig;
    }

    private async void DeleteConfigMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var config = (sender as MenuItem).DataContext as FrpConfigBase;
        if (await this.GetWindow().ShowYesNoDialogAsync("删除配置", $"是否删除配置“{config.Name}”？")==true)
        {
            if (config.ProcessStatus == ProcessStatus.Running)
            {
                await config.StopAsync();
            }
            (DataContext as MainViewModel).FrpConfigs.Remove(config);
        }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var config in (DataContext as MainViewModel).FrpConfigs)
        {
            if(config.AutoStart)
            {
                config.Start();
            }
        }
    }
}
