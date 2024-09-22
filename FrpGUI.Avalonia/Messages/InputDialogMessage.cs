using FzLib.Avalonia.Messages;

namespace FrpGUI.Avalonia.Messages
{
    public class InputDialogMessage : DialogHostMessage
    {
        public string Watermark { get; init; }

        public string DefaultText { get; init; }

        public string Message { get; init; }

        public string Title { get; init; }
    }
}