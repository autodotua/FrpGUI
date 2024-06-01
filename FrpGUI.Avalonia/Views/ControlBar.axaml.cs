using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using FzLib.Avalonia;
using FzLib.Avalonia.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class ControlBar : UserControl
{
    private MainViewModel viewModel;

    public ControlBar()
    {
        InitializeComponent();
    }
    private void AddRuleButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.Assert(viewModel.CurrentPanel is ClientPanel);
        var clientPanel = viewModel.CurrentPanel as ClientPanel;
        clientPanel.AddRule();
    }

    private async void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string config;
            FilePickerFileType filter;
            switch (AppConfig.Instance.FrpConfigType)
            {
                case "INI":
                    throw new NotImplementedException();
                case "TOML":
                    config = viewModel.CurrentFrpConfig.ToToml();
                    filter = new FilePickerFileType("ini配置文件")
                    {
                        Patterns = ["*.toml"],
                        MimeTypes = ["application/toml"]
                    };
                    break;
                default:
                    throw new Exception("未知FRP配置文件类型");
            }
            var a = TopLevel.GetTopLevel(this);
            if (await this.ShowYesNoDialogAsync("导出配置", "是否导出配置文件？", config) == true)
            {
                var file = await TopLevel.GetTopLevel(this).StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    FileTypeChoices = new[] { filter },
                    SuggestedFileName = viewModel.CurrentFrpConfig.Name,
                    DefaultExtension = filter.Patterns[0].Split('.')[1]
                });
                if (file != null)
                {
                    string path = file.TryGetLocalPath();
                    if (path != null)
                    {
                        File.WriteAllText(path, config, new UTF8Encoding(false));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await this.GetWindow().ShowErrorDialogAsync("启动失败", ex);
        }
    }

    private async void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await viewModel.CurrentFrpConfig.RestartAsync();
        }
        catch (Exception ex)
        {
            await this.GetWindow().ShowErrorDialogAsync("重启失败", ex);
        }
    }

    private async void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        await new SettingsWindow().ShowDialog(DialogExtension.ContainerType, this);
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.CurrentFrpConfig.Start();
        }
        catch (Exception ex)
        {
            await this.GetWindow().ShowErrorDialogAsync("启动失败", ex);
        }
    }
    private async void StopButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await viewModel.CurrentFrpConfig.StopAsync();
        }
        catch (Exception ex)
        {
            await this.GetWindow().ShowErrorDialogAsync("停止失败", ex);
        }
    }
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        viewModel = (this.GetVisualAncestors().OfType<MainView>().FirstOrDefault() ?? throw new System.Exception("找不到MainView"))
            .DataContext as MainViewModel;
    }
}