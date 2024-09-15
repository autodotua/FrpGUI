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
        private ObservableCollection<Process> processes;

        public SettingViewModel(IDataProvider provider,UIConfig config) : base(provider)
        {
            Config = config;
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
    }
}
