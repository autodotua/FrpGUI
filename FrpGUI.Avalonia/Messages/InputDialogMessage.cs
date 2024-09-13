using FzLib.Avalonia.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
