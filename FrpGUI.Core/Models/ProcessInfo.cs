using System.Diagnostics;

namespace FrpGUI.Models
{
    public class ProcessInfo
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public DateTime StartTime { get; set; }
        public string FileName { get; set; }

        public static List<ProcessInfo> GetFrpProcesses()
        {
            return Process.GetProcesses()
                  .Where(p => p.ProcessName is "frps" or "frpc")
                  .Select(p => new ProcessInfo()
                  {
                      Id = p.Id,
                      ProcessName = p.ProcessName,
                      StartTime = p.StartTime,
                      FileName = p.MainModule.FileName
                  })
            .ToList();
        }

        public static void KillProcess(int id)
        {
            Process process;
            try
            {
                process = Process.GetProcessById(id);
            }
            catch (ArgumentException)
            {
                throw new KeyNotFoundException($"不存在ID为{id}的进程");
            }
            if (process.ProcessName is not ("frps" or "frpc"))
            {
                throw new Exception("指定的进程不是Frp进程");
            }

            process.Kill();
        }
    }
}