﻿using FrpGUI.Models;
using FrpGUI.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrpGUI.WebAPI.Controllers;

[NeedToken]
[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly FrpDbContext db;

    public LogController(FrpDbContext db)
    {
        this.db = db;
    }

    [HttpGet("List")]
    public async Task<IList<LogEntity>> GetAsync(DateTime timeAfter)
    {
        var logs = await db.Logs
              .Where(p => p.ProcessStartTime == LogEntity.CurrentProcessStartTime)
              .Where(p => p.Time > timeAfter)
              .OrderByDescending(p => p.Time)
              .Take(100)
              .ToListAsync();
        logs.Reverse();
        return logs;
    }
}