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

public partial class SettingsWindow : DialogHost
{
    public SettingsWindow()
    {
        DataContext = new SettingWindowViewModel();
        InitializeComponent();
    }

    protected override void OnCloseButtonClick()
    {
        Close();
    }

    private void KillButton_Click(object sender, RoutedEventArgs e)
    {
       
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
}
