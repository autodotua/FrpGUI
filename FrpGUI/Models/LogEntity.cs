using FrpGUI.Configs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FrpGUI.Models
{
    public class LogEntity
    {
        public LogEntity()
        {
        }

        public LogEntity(string message, char type, FrpConfigBase config, bool fromFrp, Exception exception) : this()
        {
            Message = message;
            InstanceName = config?.Name;
            InstanceId = config?.ID;
            Type = type;
            FromFrp = fromFrp;
            Exception = exception?.ToString();
        }

        [Key]
        public int Id { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Message { get; set; }
        public string InstanceName { get; set; }
        public string InstanceId { get; set; }
        public char Type { get; set; }
        public bool FromFrp { get; set; }
        public string Exception { get; set; }
        public DateTime ProcessStartTime { get; set; } = CurrentProcessStartTime;

        public static DateTime CurrentProcessStartTime { get; } 

        static LogEntity()
        {
            //wasm不支持Process
            try
            {
                CurrentProcessStartTime = Process.GetCurrentProcess().StartTime;
            }
            catch
            {

            }
        }
    }
}