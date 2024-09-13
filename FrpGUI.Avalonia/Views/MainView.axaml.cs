using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.Messages;
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
            if ((DataContext as MainViewModel).CurrentFrpProcess == null)
            {
                mainPanel.Content = null;
            }
            else if ((DataContext as MainViewModel).CurrentFrpProcess.Config is ServerConfig)
            {
                mainPanel.Content = new ServerPanel();
            }
            else
            {
                mainPanel.Content = new ClientPanel();
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

    }

    private void RegisterMessages()
    {
        this.RegisterCommonDialogMessage();
        this.RegisterDialogHostMessage();
        this.RegisterGetClipboardMessage();
        this.RegisterGetStorageProviderMessage();
        WeakReferenceMessenger.Default.Register<InputDialogMessage>(this, async (_, m) =>
        {
            try
            {
                var result = await this.ShowInputTextDialogAsync(m.Title, m.Message, m.DefaultText, m.Watermark);
                m.SetResult(result);
            }
            catch (Exception exception)
            {
                m.SetException(exception);
            }
        });
    }
}
