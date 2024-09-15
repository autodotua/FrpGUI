using FrpGUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia
{
    public class Logger : LoggerBase
    {
        private ConcurrentBag<LogEntity> logs = new ConcurrentBag<LogEntity>();

        protected override void AddLog(LogEntity logEntity)
        {
            logs.Add(logEntity);
        }

        public IList<LogEntity> GetLogs()
        {
            try
            {
                return logs.OrderBy(p => p.Time).ToList();
            }
            finally
            {
                logs.Clear();
            }
        }
    }
}
