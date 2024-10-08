﻿using FrpGUI.Models;
using FrpGUI.Services;
using FrpGUI.WebAPI.Models;

namespace FrpGUI.WebAPI;

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