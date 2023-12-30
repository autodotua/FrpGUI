using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrpGUI.Util;

namespace FrpGUI
{
    public static class Logger
    {
        public static event EventHandler<LogEventArgs> NewLog;

        public static void Error(string message) => Log(message, 'E');

        public static void Info(string message) => Log(message, 'I');

        public static void Ouput(string message) => Log(message, 'O');

        public static void Warn(string message) => Log(message, 'W');

        private static void Log(string message, char type) => NewLog?.Invoke(null, new LogEventArgs(message, type));
    }
}
