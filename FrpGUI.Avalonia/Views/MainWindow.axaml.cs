﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using FzLib.Avalonia.Controls;
using System;

namespace FrpGUI.Avalonia.Views;

public partial class MainWindow : ExtendedWindow
{
    public MainWindow()
    {
        InitializeComponent();
        if (OperatingSystem.IsWindows())
        {
            grid.Children.Add(new WindowButtons());
        }
    }

    public MainViewModel GetDataContext()
    {
        return mainView?.DataContext as MainViewModel;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        //Config.AppConfig.Instance.Save();

        //if (GetDataContext() != null && GetDataContext().FrpConfigs.Any(p => p.ProcessStatus == ProcessStatus.Running))
        //{
        //    e.Cancel = true;
        //    if (AppConfig.Instance.ShowTrayIcon)
        //    {
        //        TrayIcon.GetIcons(App.Current)[0].IsVisible = true;
        //    }
        //    Hide();
        //}
        base.OnClosing(e);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        //if (startup)
        //{
        //    Hide();
        //    if (AppConfig.Instance.ShowTrayIcon)
        //    {
        //        TrayIcon.GetIcons(App.Current)[0].IsVisible = true;
        //    }
        //}
    }
}