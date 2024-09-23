using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Enums;
using FrpGUI.Models;
using FzLib.Avalonia.Controls;
using FzLib.Avalonia.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class MainWindow : ExtendedWindow
{
    private readonly UIConfig config;

    private readonly FrpProcessCollection processes;

    private bool forceClose = false;

    public MainWindow(UIConfig config, FrpProcessCollection processes = null)
    {
        InitializeComponent();
        if (OperatingSystem.IsWindows())
        {
            grid.Children.Add(new WindowButtons());
        }

        this.config = config;
        this.processes = processes;
    }

    public async Task TryCloseAsync()
    {
        if (config.RunningMode == RunningMode.Service)
        {
            forceClose = true;
            Close();
        }

        Debug.Assert(processes != null);
        if (processes.Any(p => p.Value.ProcessStatus == ProcessStatus.Running))
        {
            if (!IsVisible)
            {
                Show();
            }
            var runningFrps = processes.Where(p => p.Value.ProcessStatus == ProcessStatus.Running).ToList();
            if (await this.ShowYesNoDialogAsync("退出", $"存在{runningFrps.Count}个正在运行的frp进程，是否退出？") == true)
            {
                foreach (var frp in runningFrps)
                {
                    await frp.Value.StopAsync();
                }
                forceClose = true;
                Close();
            }
        }
        else
        {
            forceClose = true;
            Close();
        }
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        if (forceClose)
        {
            return;
        }
        if (config.ShowTrayIcon)
        {
            e.Cancel = true;
            Hide();
        }
        else
        {
            await TryCloseAsync();
        }
    }
}