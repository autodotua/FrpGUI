using FrpGUI.Models;
using FrpGUI.Services;
using System;

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