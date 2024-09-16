using FrpGUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia
{
    public class LocalLogger : LoggerBase
    {
        public event EventHandler<NewLogEventArgs> NewLog;

        protected override void AddLog(LogEntity logEntity)
        {
            NewLog?.Invoke(this, new NewLogEventArgs(logEntity));
        }

        public class NewLogEventArgs : EventArgs
        {
            public NewLogEventArgs(LogEntity log)
            {
                ArgumentNullException.ThrowIfNull(log);
                Log = log;
            }

            public LogEntity Log { get; }
        }
    }
}
