using FrpGUI.Models;
using System.Text.RegularExpressions;

namespace FrpGUI.Services
{
    public abstract class LoggerBase
    {
        private readonly string[] errorMessages = [
            "error",
            "unknown",
            "Only one usage of each socket address (protocol/network address/port) is normally permitted.",
        ];

        private readonly Regex rFrpLog = new Regex(@"(?<Time>[[0-9:\.\- ]{23}) \[(?<Type>.)\] \[[^\]]+\] (?<Content>.*)", RegexOptions.Compiled);

        public void Error(string message, FrpConfigBase config = null, Exception ex = null) => Log(message, 'E', config, false, ex);

        public void Info(string message, FrpConfigBase config = null) => Log(message, 'I', config);

        public void Output(string message, FrpConfigBase config = null)
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
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            Log(message, type, config, true);
        }

        public void Warn(string message, FrpConfigBase config = null) => Log(message, 'W', config);

        protected abstract void AddLog(LogEntity logEntity);

        private void Log(string message, char type, FrpConfigBase config, bool fromFrp = false, Exception ex = null)
        {
            AddLog(new LogEntity(message, type, config, fromFrp, ex));
        }
    }
}