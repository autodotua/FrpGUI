using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using FzLib.Avalonia.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        DataContext = new SettingWindowViewModel();
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        IsEnabled = false;
        try
        {
            List<Process> processes = null;
            await Task.Run(() =>
            {
                processes = Process.GetProcesses()
                .Where(p => p.ProcessName is "frps" or "frpc")
                .ToList();
            });
            (DataContext as SettingWindowViewModel).Processes =
                new ObservableCollection<Process>(processes);
        }
        finally
        {
            IsEnabled = true;
        }
    }

    private void KillButton_Click(object sender, RoutedEventArgs e)
    {
        Process p = (sender as Button).DataContext as Process;
        Debug.Assert(p != null);
        try
        {
            p.Kill();
            (DataContext as SettingWindowViewModel).Processes.Remove(p);
        }
        catch (Exception ex)
        {
            (Content as Grid).ShowErrorDialogAsync("停止进程失败", ex, true);
        }
    }

    private async void RemoteControlEnableSwitch_Checked(object sender, RoutedEventArgs e)
    {
        if (!(App.Current as App).HttpServerHelper.IsRunning)
        {
            await (App.Current as App).HttpServerHelper.StartAsync();
        }
    }
    private void RemoteControlEnableSwitch_Unchecked(object sender, RoutedEventArgs e)
    {
        if ((App.Current as App).HttpServerHelper.IsRunning)
        {
            (App.Current as App).HttpServerHelper.Stop();
        }
    }
}
