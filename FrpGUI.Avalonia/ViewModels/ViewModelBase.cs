using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.DataProviders;
using static CommunityToolkit.Mvvm.Messaging.IMessengerExtensions;

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
}
