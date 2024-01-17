using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using MsBox.Avalonia;
using MsBox.Avalonia.Models;
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
        if (await ShowYesNoAsync("删除配置", $"是否删除配置“{config.Name}”？"))
        {
            if (config.ProcessStatus == ProcessStatus.Running)
            {
                await config.StopAsync();
            }
            (DataContext as MainViewModel).FrpConfigs.Remove(config);
        }
    }
    private async Task<bool> ShowYesNoAsync(string title, string message)
    {
        //暂时先用一下这些不好用的MessageBox
        return await MessageBoxManager.GetMessageBoxCustom(new MsBox.Avalonia.Dto.MessageBoxCustomParams()
        {
            ContentTitle = title,
            ContentMessage = message,
            FontFamily = this.FontFamily,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ButtonDefinitions = new List<ButtonDefinition>
                {
                    new ButtonDefinition { Name = "是", IsDefault=true},
                    new ButtonDefinition { Name = "否", IsCancel=true},
                },
        }).ShowAsync() == "是";
    }
}
