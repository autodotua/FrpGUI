using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FrpGUI.Util;

namespace FrpGUI
{
    public static class Logger
    {
        private static Regex rFrpLog = new Regex(@"(?<Time>[0-9/: ]{19}) \[(?<Type>.)\] \[[^\]]+\] (?<Content>.*)", RegexOptions.Compiled);

        public static event EventHandler<LogEventArgs> NewLog;

        public static void Error(string message, string instanceName = null) => Log(message, instanceName, 'E');

        public static void Info(string message, string instanceName = null) => Log(message, instanceName, 'I');

        public static void Ouput(string message, string instanceName)
        {
            char type;
            if (rFrpLog.IsMatch(message))
            {
                var match = rFrpLog.Match(message);
                message = match.Groups["Content"].Value;
                type = match.Groups["Type"].Value[0];
            }
            else
            {
                type = message.Contains("error") || message.Contains("unknown") ? 'E' : 'I';
            }
            Log(message, instanceName, type, true);
        }

        public static void Warn(string message, string instanceName = null) => Log(message, instanceName, 'W');

        private static void Log(string message, string instanceName, char type, bool fromFrp = false) => NewLog?.Invoke(null, new LogEventArgs(message, instanceName, type, fromFrp));
    }
}
