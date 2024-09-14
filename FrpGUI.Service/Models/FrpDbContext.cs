﻿using FrpGUI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FrpGUI.Service.Models
{
    public class FrpDbContext : DbContext
    {
        private static readonly string connectionString;

        static FrpDbContext()
        {
          
            connectionString = $"Data Source=logs.db";
        }
        public FrpDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<LogEntity> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}