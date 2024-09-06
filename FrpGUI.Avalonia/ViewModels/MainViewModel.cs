using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrpGUI.Avalonia.DataProviders;
using FrpGUI.Avalonia.Models;
using FrpGUI.Avalonia.Views;
using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using FzLib.Avalonia.Dialogs;
using FzLib.Avalonia.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FzLib.Avalonia.Messages.CommonDialogMessage;

namespace FrpGUI.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IDataProvider provider;

    private readonly IServiceProvider services;

    [ObservableProperty]
    private IFrpProcess currentFrpProcess;

    [ObservableProperty]
    private FrpConfigViewModel currentPanelViewModel;

    [ObservableProperty]
    private ObservableCollection<IFrpProcess> frpProcesses = new ObservableCollection<IFrpProcess>();

    public MainViewModel(IDataProvider provider,
                         IServiceProvider services,
                         FrpConfigViewModel frpConfigViewModel) : base(provider)
    {
        this.services = services;
        InitializeDataAndStartTimer();
        CurrentPanelViewModel = frpConfigViewModel;
    }

    [RelayCommand]
    private async Task AddClientAsync()
    {
        var newConfig = await DataProvider.AddClientAsync();
        var fp = new FrpProcess(newConfig);
        FrpProcesses.Add(fp);
        CurrentFrpProcess = fp;
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
        var fp = new FrpProcess(newConfig);
        FrpProcesses.Add(fp);
        CurrentFrpProcess = fp;
    }

    [RelayCommand]
    private void CreateCopy(FrpConfigBase config)
    {
        //var newConfig = config.Clone() as FrpConfigBase;
        //newConfig.Name = newConfig.Name + "（复制）";
        //FrpConfigs.Add(newConfig);
        //CurrentFrpProcess = newConfig;
    }

    [RelayCommand]
    private async Task DeleteConfigAsync(IFrpProcess fp)
    {
        var message = SendMessage(new CommonDialogMessage()
        {
            Type = CommonDialogType.YesNo,
            Title = "删除配置",
            Message = $"是否删除配置“{fp.Config.Name}”？"
        });
        if (true.Equals(await message.Task))
        {
            FrpProcesses.Remove(fp);
            await DataProvider.DeleteFrpConfigAsync(fp.Config.ID);
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        try
        {
            string config;
            FilePickerFileType filter;

            config = CurrentFrpProcess.Config.ToToml();
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
                    SuggestedFileName = CurrentFrpProcess.Config.Name,
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

    private async void InitializeDataAndStartTimer()
    {
        try
        {
            FrpProcesses = new ObservableCollection<IFrpProcess>(await DataProvider.GetFrpStatusesAsync());
        }
        catch (Exception ex)
        {
            SendMessage(new CommonDialogMessage()
            {
                Type = CommonDialogType.Error,
                Title = "获取配置列表失败",
                Exception = ex
            });
        }
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync())
        {
            try
            {
                await UpdateStatusAsync(false);
            }
            catch (Exception ex)
            {

            }
        }
    }

    partial void OnCurrentFrpProcessChanged(IFrpProcess value)
    {
        CurrentPanelViewModel.LoadConfig(value);
    }

    partial void OnCurrentFrpProcessChanging(IFrpProcess oldValue, IFrpProcess newValue)
    {
        if (oldValue != null)
        {
            DataProvider.ModifyConfigAsync(oldValue.Config);
        }
    }
    [RelayCommand]
    private async Task RestartAsync()
    {
        try
        {
            await DataProvider.ModifyConfigAsync(CurrentFrpProcess.Config);
            await DataProvider.RestartFrpAsync(CurrentFrpProcess.Config.ID);
            //await   WaitForNextUpdate();
            await UpdateStatusAsync(true);
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
            await DataProvider.ModifyConfigAsync(CurrentFrpProcess.Config);
            await DataProvider.StartFrpAsync(CurrentFrpProcess.Config.ID);
            await UpdateStatusAsync(true);
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
            await DataProvider.StopFrpAsync(CurrentFrpProcess.Config.ID);
            await UpdateStatusAsync(true);
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

    TaskCompletionSource tcsUpdate;
    public Task WaitForNextUpdate()
    {
        tcsUpdate = new TaskCompletionSource();
        return tcsUpdate.Task;
    }
    private async Task UpdateStatusAsync(bool force)
    {
        if (!force && (DateTime.Now - lastUpdateStatusTime).TotalSeconds < 1)
        {
            return;
        }
        lastUpdateStatusTime = DateTime.Now;
        var fps = await DataProvider.GetFrpStatusesAsync();
        var local = FrpProcesses.ToDictionary(p => p.Config.ID);
        foreach (var fp in fps)
        {
            if (local.TryGetValue(fp.Config.ID, out var localFp))
            {
                localFp.ProcessStatus = fp.ProcessStatus;
            }
        }
        if (tcsUpdate != null)
        {
            tcsUpdate.SetResult();
            tcsUpdate = null;
        }
    }

    private DateTime lastUpdateStatusTime = DateTime.MinValue;
}
