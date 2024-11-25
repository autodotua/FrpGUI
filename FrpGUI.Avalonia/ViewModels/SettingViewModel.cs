﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrpGUI.Avalonia.DataProviders;
using FrpGUI.Models;
using FzLib.Avalonia.Messages;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using FzLib.Program.Startup;

namespace FrpGUI.Avalonia.ViewModels
{
    public partial class SettingViewModel : ViewModelBase
    {
        private readonly IStartupManager startupManager;

        [ObservableProperty]
        private string newToken;

        [ObservableProperty]
        private string oldToken;

        [ObservableProperty]
        private ObservableCollection<ProcessInfo> processes;

        [ObservableProperty]
        private string serverAddress;

        [ObservableProperty]
        private bool startup;

        public SettingViewModel(IDataProvider provider, UIConfig config,IStartupManager startupManager) : base(provider)
        {
            this.startupManager = startupManager;
            startup = startupManager.IsStartupEnabled();
            Config = config;
            ServerAddress = config.ServerAddress;
            FillProcesses();
            Config.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Config.RunningMode) && Config.RunningMode == RunningMode.Service)
                {
                    Startup = false;
                }
            };
        }

        partial void OnStartupChanged(bool value)
        {
            if (value)
            {
                startupManager.EnableStartup("s");
                Config.ShowTrayIcon = true;
            }
            else
            {
                startupManager.DisableStartup();
            }
        }
        
        private async void FillProcesses()
        {
            try
            {
                Processes = new ObservableCollection<ProcessInfo>(await DataProvider.GetSystemProcesses());
            }
            catch (Exception ex)
            { }
        }

        public UIConfig Config { get; }

        [RelayCommand]
        private async Task KillProcessAsync(ProcessInfo p)
        {
            Debug.Assert(p != null);
            try
            {
                await DataProvider.KillProcess(p.Id);
                Processes.Remove(p);
            }
            catch (Exception ex)
            {
                await SendMessage(new CommonDialogMessage()
                {
                    Type = CommonDialogMessage.CommonDialogType.Error,
                    Title = "结束进程失败",
                    Exception = ex,
                }).Task;
            }
        }

        [RelayCommand]
        private async Task RestartAsync()
        {
            Config.ServerAddress = ServerAddress;
            if (!string.IsNullOrEmpty(NewToken))
            {
                Config.ServerToken = NewToken;
            }
            Config.Save();

            if (OperatingSystem.IsBrowser())
            {
                JsInterop.Reload();
            }
            else
            {
                string exePath = Environment.ProcessPath;
                Process.Start(new ProcessStartInfo(exePath)
                {
                    UseShellExecute = true
                });
                await (App.Current as App).ShutdownAsync();
            }
        }

        [RelayCommand]
        private async Task SetTokenAsync()
        {
            try
            {
                Config.ServerAddress = ServerAddress;
                await DataProvider.SetTokenAsync(OldToken, NewToken);
                Config.ServerToken = NewToken;
                Config.Save();
                await SendMessage(new CommonDialogMessage()
                {
                    Type = CommonDialogMessage.CommonDialogType.Ok,
                    Title = "修改密码",
                    Message = "修改密码成功"
                }).Task;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex, "修改密码失败");
            }
        }
    }
}