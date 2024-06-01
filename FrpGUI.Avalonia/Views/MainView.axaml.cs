using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.Messages;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using FzLib.Avalonia;
using FzLib.Avalonia.Dialogs;
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
        DataContext = new MainViewModel();
        InitializeComponent();
        RegisterMessages();
        (DataContext as MainViewModel).PropertyChanged += MainView_PropertyChanged;
    }

    private void MainView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.CurrentFrpConfig))
        {
            if ((DataContext as MainViewModel).CurrentFrpConfig is ServerConfig)
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
        WeakReferenceMessenger.Default.Register<DialogHostMessage>(this, async (_, m) =>
        {
            try
            {
                var result = await m.Dialog.ShowDialog<object>(DialogContainerType.PopupPreferred, TopLevel.GetTopLevel(this));
                m.SetResult(result);
            }
            catch (Exception ex)
            {
                m.SetException(ex);
            }
        });

        WeakReferenceMessenger.Default.Register<StorageProviderMessage>(this, (_, m) =>
        {
            m.StorageProvider = TopLevel.GetTopLevel(this).StorageProvider;
        });

        WeakReferenceMessenger.Default.Register<ClipboardMessage>(this, (_, m) =>
        {
            m.Clipboard = TopLevel.GetTopLevel(this).Clipboard;
        });

        WeakReferenceMessenger.Default.Register<CommonDialogMessage>(this, async (_, m) =>
        {
            try
            {
                object result = null;
                switch (m.Type)
                {
                    case CommonDialogMessage.CommonDialogType.Ok:
                       await this.ShowOkDialogAsync(m.Title, m.Message, m.Detail);
                        break;
                    case CommonDialogMessage.CommonDialogType.Error:
                        if (m.Exception == null)
                        {
                            result =await this.ShowErrorDialogAsync(m.Title, m.Message, m.Detail);
                        }
                        else
                        {
                            result =await this.ShowErrorDialogAsync(m.Title, m.Exception);
                        }
                        break;
                    case CommonDialogMessage.CommonDialogType.YesNo:
                        result =await this.ShowYesNoDialogAsync(m.Title, m.Message, m.Detail);
                        break;
                }
                m.SetResult(result);
            }
            catch (Exception ex)
            {
                m.SetException(ex);
            }
        });
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var config in (DataContext as MainViewModel).FrpConfigs.Where(p => p.AutoStart))
        {
            try
            {
                config.Start();
            }
            catch
            {

            }
        }
    }
}
