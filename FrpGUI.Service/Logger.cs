using FrpGUI;
using FrpGUI.Configs;
using FrpGUI.Models;
using FrpGUI.Service.Models;
using System.Text.RegularExpressions;

public class Logger : LoggerBase
{
    private readonly FrpDbContext db;

    private PeriodicTimer timer;

    public Logger(FrpDbContext db)
    {
        this.db = db;
        StartTimer();
    }

    protected override void AddLog(LogEntity logEntity)
    {
        db.Logs.Add(logEntity);
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
            catch (Exception ex)
            {

            }
        }
    }
}
