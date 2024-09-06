using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrpGUI.Avalonia.DataProviders;
using FrpGUI.Avalonia.Views;
using FrpGUI.Configs;
using FrpGUI.Enums;
using FzLib.Avalonia.Dialogs;
using FzLib.Avalonia.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static FzLib.Avalonia.Messages.CommonDialogMessage;

namespace FrpGUI.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private FrpConfigBase currentFrpConfig;

    [ObservableProperty]
    private FrpConfigViewModel currentPanelViewModel;

    [ObservableProperty]
    private ObservableCollection<FrpConfigBase> frpConfigs = new ObservableCollection<FrpConfigBase>();

    partial void OnCurrentFrpConfigChanging(FrpConfigBase oldValue, FrpConfigBase newValue)
    {
        if (oldValue != null)
        {
            DataProvider.ModifyConfigAsync(oldValue);
        }
    }
    private readonly IDataProvider provider;
    private readonly IServiceProvider services;

    public MainViewModel(IDataProvider provider,
                         IServiceProvider services,
                         FrpConfigViewModel frpConfigViewModel) : base(provider)
    {
        this.services = services;
        InitializeData();
        CurrentPanelViewModel = frpConfigViewModel;
    }

    private async void InitializeData()
    {
        try
        {
            FrpConfigs = new ObservableCollection<FrpConfigBase>(await DataProvider.GetFrpConfigsAsync());
        }
        catch (Exception ex)
        {
            this.SendMessage(new CommonDialogMessage()
            {
                Type = CommonDialogType.Error,
                Title = "获取配置列表失败",
                Exception = ex
            });
        }

    }

    [RelayCommand]
    private async Task AddClientAsync()
    {
        var newConfig = await DataProvider.AddClientAsync();
        FrpConfigs.Add(newConfig);
        CurrentFrpConfig = newConfig;
    }

    [RelayCommand]
    private async Task AddRuleAsync()
    {
        await CurrentPanelViewModel.AddRuleAsync();
    }

    [RelayCommand]
    private async Task AddServerAsync()
    {
        var newConfig = await DataProvider.AddServerAsync();
        FrpConfigs.Add(newConfig);
        CurrentFrpConfig = newConfig;
    }

    [RelayCommand]
    private void CreateCopy(FrpConfigBase config)
    {
        var newConfig = config.Clone() as FrpConfigBase;
        newConfig.Name = newConfig.Name + "（复制）";
        FrpConfigs.Add(newConfig);
        CurrentFrpConfig = newConfig;
    }

    [RelayCommand]
    private async Task DeleteConfigAsync(FrpConfigBase config)
    {
        var message = SendMessage(new CommonDialogMessage()
        {
            Type = CommonDialogType.YesNo,
            Title = "删除配置",
            Message = $"是否删除配置“{config.Name}”？"
        });
        if (true.Equals(await message.Task))
        {
            FrpConfigs.Remove(config);
            await DataProvider.DeleteFrpConfigAsync(config.ID);
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        try
        {
            string config;
            FilePickerFileType filter;

            config = CurrentFrpConfig.ToToml();
            filter = new FilePickerFileType("ini配置文件")
            {
                Patterns = ["*.toml"],
                MimeTypes = ["application/toml"]
            };
            var message = SendMessage(new CommonDialogMessage()
            {
                Type = CommonDialogType.YesNo,
                Title = "导出配置",
                Message = "是否导出配置文件？",
                Detail = config
            });

            if (true.Equals(await message.Task))
            {
                var file = await SendMessage(new GetStorageProviderMessage()).StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    FileTypeChoices = [filter],
                    SuggestedFileName = CurrentFrpConfig.Name,
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
            await SendMessage(new CommonDialogMessage()
            {
                Type = CommonDialogType.Error,
                Title = "启动失败",
                Exception = ex
            }).Task;
        }
    }

    partial void OnCurrentFrpConfigChanged(FrpConfigBase value)
    {
        CurrentPanelViewModel.LoadConfig(CurrentFrpConfig);
    }
    [RelayCommand]
    private async Task RestartAsync()
    {
        try
        {
            await DataProvider.RestartFrpAsync(CurrentFrpConfig.ID);
        }
        catch (Exception ex)
        {
            await SendMessage(new CommonDialogMessage()
            {
                Type = CommonDialogType.Error,
                Title = "重启失败",
                Exception = ex
            }).Task;
        }
    }

    [RelayCommand]
    private async Task SettingsAsync()
    {
        await SendMessage(new DialogHostMessage(services.GetRequiredService<SettingsWindow>())).Task;
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        try
        {
            await DataProvider.ModifyConfigAsync(CurrentFrpConfig);
            await DataProvider.StartFrpAsync(CurrentFrpConfig.ID);
        }
        catch (Exception ex)
        {
            await SendMessage(new CommonDialogMessage()
            {
                Type = CommonDialogType.Error,
                Title = "启动失败",
                Exception = ex
            }).Task;
        }
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        try
        {
            await DataProvider.StopFrpAsync(CurrentFrpConfig.ID);
        }
        catch (Exception ex)
        {
            await SendMessage(new CommonDialogMessage()
            {
                Type = CommonDialogType.Error,
                Title = "停止失败",
                Exception = ex
            }).Task;
        }
    }
}
