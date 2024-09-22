using FrpGUI.Models;
using FrpGUI.Services;
using FrpGUI.WebAPI.Models;

namespace FrpGUI.WebAPI;

public class WebLogger : LoggerBase
{
    private readonly FrpDbContext db;
    private readonly ILogger sysLogger;
    private PeriodicTimer timer;

    public WebLogger(FrpDbContext db)
    {
        this.db = db;
        StartTimer();
    }

    protected override void AddLog(LogEntity logEntity)
    {
        db.Logs.Add(logEntity);
        LogLevel logLevel = logEntity.Type switch
        {
            'I' => LogLevel.Information,
            'E' => LogLevel.Error,
            'W' => LogLevel.Warning,
            _ => LogLevel.None
        };

        //sysLogger.Log(logLevel, "（{InstanceName}）{Message}", logEntity.InstanceName ?? "无", logEntity.Message);
        Console.WriteLine($"{logEntity.Time}\t{logEntity.Type}\t{logEntity.InstanceName ?? "  "}\t{logEntity.Message}");
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