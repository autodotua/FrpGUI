using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaWebView;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.Messages;
using FrpGUI.Avalonia.ViewModels;

using FrpGUI.Models;
using FzLib.Avalonia.Dialogs;
using FzLib.Avalonia.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FrpGUI.Avalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        DataContext = App.Services.GetRequiredService<MainViewModel>();
        InitializeComponent();
        RegisterMessages();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (TopLevel.GetTopLevel(this) is Window win)
        {
            foreach (var control in new Control[] { controlBar, tbkLogo })
            {

                control.PointerPressed += (s, e) =>
                {
                    win.BeginMoveDrag(e);
                };

                control.DoubleTapped += (s, e) =>
                {
                    win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                };
            }
        }
    }

    private void RegisterDialogHostMessage()
    {
        WeakReferenceMessenger.Default.Register(this, async delegate (object _, DialogHostMessage m)
        {
            try
            {
                m.SetResult(await m.Dialog.ShowDialog<object>(DialogContainerType.WindowPreferred, TopLevel.GetTopLevel(this)));
            }
            catch (Exception exception)
            {
                m.SetException(exception);
            }
        });
    }
    private void RegisterMessages()
    {
        this.RegisterCommonDialogMessage();
        RegisterDialogHostMessage();
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