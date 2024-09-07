using FrpGUI.Configs;
using FrpGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrpGUI
{
    public class Logger
    {
        private readonly FrpDbContext db;

        private readonly string[] errorMessages = [
            "error",
            "unknown",
            "Only one usage of each socket address (protocol/network address/port) is normally permitted.",
        ];

        private readonly Regex rFrpLog = new Regex(@"(?<Time>[[0-9:\.\- ]{23}) \[(?<Type>.)\] \[[^\]]+\] (?<Content>.*)", RegexOptions.Compiled);
        PeriodicTimer timer;

        public Logger(FrpDbContext db)
        {
            this.db = db;
            StartTimer();
        }
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
            Log(message, type, config, true);
        }

        public void Warn(string message, FrpConfigBase config = null) => Log(message, 'W', config);

        private void Log(string message, char type, FrpConfigBase config, bool fromFrp = false, Exception ex = null)
        {
            db.Add(new LogEntity(message, type, config, fromFrp, ex));
        }

        private async void StartTimer()
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (await timer.WaitForNextTickAsync())
            {
                try
                {
                    await db.SaveChangesAsync();
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}
