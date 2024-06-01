using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using static CommunityToolkit.Mvvm.Messaging.IMessengerExtensions;

namespace FrpGUI.Avalonia.ViewModels;

public class ViewModelBase : ObservableObject
{
    protected TMessage SendMessage<TMessage>(TMessage message) where TMessage : class
    {
        return WeakReferenceMessenger.Default.Send(message);
    }
}
