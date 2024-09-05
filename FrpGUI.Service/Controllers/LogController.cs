﻿using FrpGUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrpGUI.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    //private const string lastGetLogsTimeKey = "lastGetLogsTime";
    private readonly FrpDbContext db;

    public LogController(FrpDbContext db)
    {
        this.db = db;
    }

    [HttpGet("List")]
    public async Task<IList<LogEntity>> GetAsync(DateTime timerAfter)
    {
        //DateTime time = DateTime.MinValue;
        //var timeString = HttpContext.Session.GetString(lastGetLogsTimeKey);
        //if (timeString != null)
        //{
        //    time = DateTime.Parse(timeString);
        //}
        //HttpContext.Session.SetString(lastGetLogsTimeKey, time.ToString());
        var logs = await db.Logs
              .Where(p => p.ProcessStartTime == LogEntity.CurrentProcessStartTime)
              .Where(p => p.Time > timerAfter)
              .OrderByDescending(p => p.Time)
              .Take(100)
              .ToListAsync();
        logs.Reverse();
        return logs;
    }
}
