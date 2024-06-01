using Avalonia.Platform.Storage;

namespace FrpGUI.Avalonia.Messages
{
    public class StorageProviderMessage()
    {
        public IStorageProvider StorageProvider { get; set; }
    }
}
