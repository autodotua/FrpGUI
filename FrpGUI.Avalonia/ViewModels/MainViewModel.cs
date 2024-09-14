using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrpGUI.Avalonia.DataProviders;
using FrpGUI.Avalonia.Messages;
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
    private readonly AppConfig config;
    private readonly IServiceProvider services;

    [ObservableProperty]
    private bool activeProgressRingOverlay = true;

    [ObservableProperty]
    private IFrpProcess currentFrpProcess;

    [ObservableProperty]
    private FrpConfigViewModel currentPanelViewModel;

    [ObservableProperty]
    private ObservableCollection<IFrpProcess> frpProcesses = new ObservableCollection<IFrpProcess>();

    public MainViewModel(IDataProvider provider,
                         AppConfig config,
                         IServiceProvider services,
                         FrpConfigViewModel frpConfigViewModel) : base(provider)
    {
        this.config = config;
        this.services = services;
        InitializeDataAndStartTimer();
        CurrentPanelViewModel = frpConfigViewModel;
    }

    [RelayCommand]
    private async Task AddClientAsync()
    {
        try
        {
            var newConfig = await DataProvider.AddClientAsync();
            var fp = new FrpProcess(newConfig);
            FrpProcesses.Add(fp);
            CurrentFrpProcess = fp;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, "新增客户端失败");
        }
    }

    [RelayCommand]
    private async Task AddRuleAsync()
    {
        await CurrentPanelViewModel.AddRuleAsync();
    }

    [RelayCommand]
    private async Task AddServerAsync()
    {
        try
        {
            var newConfig = await DataProvider.AddServerAsync();
            var fp = new FrpProcess(newConfig);
            FrpProcesses.Add(fp);
            CurrentFrpProcess = fp;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, "新增客户端失败");
        }
    }

    [RelayCommand]
    private async Task CreateCopyAsync(IFrpProcess fp)
    {
        //var newConfig = config.Clone() as FrpConfigBase;
        //newConfig.Name = newConfig.Name + "（复制）";
        //FrpConfigs.Add(newConfig);
        //CurrentFrpProcess = newConfig;
        try
        {
            FrpConfigBase serverConfig;
            //在服务器只是新增获取一个ID号，在本地克隆替换ID号提交到服务器
            if (fp.Config is ClientConfig)
            {
                serverConfig = await DataProvider.AddClientAsync();
            }
            else if (fp.Config is ServerConfig)
            {
                serverConfig = await DataProvider.AddServerAsync();
            }
            else
            {
                throw new Exception("未知的当前选择的配置类型");
            }
            var newConfig = fp.Config.Clone() as FrpConfigBase;
            newConfig.ID = serverConfig.ID;
            await DataProvider.ModifyConfigAsync(newConfig);

            var newFp = new FrpProcess(newConfig);
            FrpProcesses.Add(newFp);
            CurrentFrpProcess = newFp;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, "创建副本失败");
        }
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
            try
            {
                await DataProvider.DeleteFrpConfigAsync(fp.Config.ID);
                FrpProcesses.Remove(fp);
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex, "删除失败");
            }
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
            await ShowErrorAsync(ex, "启动失败");
        }
    }

    private async Task CheckNetworkAndToken()
    {
    start:
        try
        {
            var result = await DataProvider.VerifyTokenAsync();
            string token;
            switch (result)
            {
                case TokenVerification.OK:
                    return;

                case TokenVerification.NotEqual:
                    do
                    {
                        token = await SendMessage(new InputDialogMessage()
                        {
                            Title = "验证密码",
                            Message = "密码不正确，请重新输入密码",
                        }).Task as string;
                        config.Token = token;
                        config.Save();
                    } while (await DataProvider.VerifyTokenAsync() != TokenVerification.OK);
                    break;

                case TokenVerification.NeedSet:
                    do
                    {
                        token = await SendMessage(new InputDialogMessage()
                        {
                            Title = "设置密码",
                            Message = "当前密码为空，需要先设置密码",
                        }).Task as string;
                    } while (string.IsNullOrWhiteSpace(token));
                    await DataProvider.SetTokenAsync("", token);
                    config.Token = token;
                    config.Save();
                    break;
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, "网络错误，无法连接到FrpGUI服务端");
            goto start;
        }
    }

    private async void InitializeDataAndStartTimer()
    {
        await CheckNetworkAndToken();
        ActiveProgressRingOverlay = false;
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
        if (oldValue != null && FrpProcesses.Contains(oldValue))
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
            await ShowErrorAsync(ex, "重启失败");
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
            await ShowErrorAsync(ex, "启动失败");
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
            await ShowErrorAsync(ex, "停止失败");
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
