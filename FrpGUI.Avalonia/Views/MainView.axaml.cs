﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Configs;
using FzLib.Avalonia;
using FzLib.Avalonia.Dialogs;
using FzLib.Avalonia.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        DataContext = App.Services.GetRequiredService<MainViewModel>();
        InitializeComponent();
        RegisterMessages();
        (DataContext as MainViewModel).PropertyChanged += MainView_PropertyChanged;
    }

    private void MainView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.CurrentFrpProcess))
        {
            if ((DataContext as MainViewModel).CurrentFrpProcess.Config is ServerConfig)
            {
                mainPanel.Content = new ServerPanel();
            }
            else
            {
                mainPanel.Content = new ClientPanel();
            }
        }
    }

    private void RegisterMessages()
    {
        this.RegisterCommonDialogMessage();
        this.RegisterDialogHostMessage();
        this.RegisterGetClipboardMessage();
        this.RegisterGetStorageProviderMessage();
    }
}
