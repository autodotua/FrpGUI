using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrpGUI.Avalonia.DataProviders;
using FrpGUI.Configs;
using FzLib.Avalonia.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.ViewModels
{
    public partial class SettingViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string newToken;

        [ObservableProperty]
        private string oldToken;

        [ObservableProperty]
        private ObservableCollection<Process> processes;

        [ObservableProperty]
        private string serverAddress;
        public SettingViewModel(IDataProvider provider, UIConfig config) : base(provider)
        {
            Config = config;
            ServerAddress = config.ServerAddress;
        }

        public UIConfig Config { get; }

        [RelayCommand]
        private async Task KillProcessAsync(Process p)
        {
            Debug.Assert(p != null);
            try
            {
                p.Kill();
                Processes.Remove(p);
            }
            catch (Exception ex)
            {
                await SendMessage(new CommonDialogMessage()
                {
                    Type = CommonDialogMessage.CommonDialogType.Error,
                    Title = "停止进程失败",
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
