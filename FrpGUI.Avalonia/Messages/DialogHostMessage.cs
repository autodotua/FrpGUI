using FzLib.Avalonia.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Messages
{
    public class DialogHostMessage
    {
        private TaskCompletionSource<object> tcs;
        public DialogHostMessage()
        {
            tcs = new TaskCompletionSource<object>();
            Task = tcs.Task;
        }

        public DialogHostMessage(DialogHost dialog) : this()
        {
            Dialog = dialog;
        }

        public DialogHost Dialog { get; }
        public Task<object> Task { get; }
        public void SetException(Exception ex)
        {
            tcs.SetException(ex);
        }

        public void SetResult(object result)
        {
            tcs.SetResult(result);
        }

        public void SetResult()
        {
            tcs.SetResult(null);
        }
    }
}
