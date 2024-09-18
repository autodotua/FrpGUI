using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.DataProviders;
using FzLib.Avalonia.Messages;
using System;
using System.Threading.Tasks;
using static CommunityToolkit.Mvvm.Messaging.IMessengerExtensions;
using static FzLib.Avalonia.Messages.CommonDialogMessage;

namespace FrpGUI.Avalonia.ViewModels;

public class ViewModelBase : ObservableObject
{
    public ViewModelBase(IDataProvider provider)
    {
        DataProvider = provider;
    }

    protected IDataProvider DataProvider { get; }

    protected TMessage SendMessage<TMessage>(TMessage message) where TMessage : class
    {
        return WeakReferenceMessenger.Default.Send(message);
    }

    protected Task ShowErrorAsync(Exception ex, string title)
    {
        return SendMessage(new CommonDialogMessage()
        {
            Type = CommonDialogType.Error,
            Title = title,
            Exception = ex
        }).Task;
    }

    protected Task ShowErrorAsync(string message, string title)
    {
        return SendMessage(new CommonDialogMessage()
        {
            Type = CommonDialogType.Error,
            Title = title,
            Message = message
        }).Task;
    }
}