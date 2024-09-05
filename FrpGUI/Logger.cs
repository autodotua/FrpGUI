using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrpGUI
{
    public static class Logger
    {
        private static readonly string[] errorMessages = [
            "error",
            "unknown",
            "Only one usage of each socket address (protocol/network address/port) is normally permitted.",
        ];

        private static readonly Regex rFrpLog = new Regex(@"(?<Time>[[0-9:\.\- ]{23}) \[(?<Type>.)\] \[[^\]]+\] (?<Content>.*)", RegexOptions.Compiled);

        public static event EventHandler<LogEventArgs> NewLog;

        public static void Error(string message, string instanceName = null, Exception ex = null) => Log(message, instanceName, 'E', false, ex);

        public static void Info(string message, string instanceName = null) => Log(message, instanceName, 'I');

        public static void Output(string message, string instanceName)
        {
            char type;
            message = Regex.Replace(message, @"\u001b\[[0-9;]*m", "");
            if (rFrpLog.IsMatch(message))
            {
                var match = rFrpLog.Match(message);
                message = match.Groups["Content"].Value;
                type = match.Groups["Type"].Value[0];
            }
            else
            {
                type = errorMessages.Any(p => message.Contains(p)) ? 'E' : 'I';
            }
            Log(message, instanceName, type, true);
        }

        public static void Warn(string message, string instanceName = null) => Log(message, instanceName, 'W');

        private static void Log(string message, string instanceName, char type, bool fromFrp = false, Exception ex = null) => NewLog?.Invoke(null, new LogEventArgs(message, instanceName, type, fromFrp, ex));
    }
}
