using System;
using System.Threading.Tasks;
using ElevatorSystem.Data;
using ElevatorSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ElevatorSystem.Services
{
    public class ElevatorLogger
    {
        private readonly string _connectionString;

        public ElevatorLogger(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task LogEventAsync(ElevatorEvent ev)
        {
            try
            {
                var options = new DbContextOptionsBuilder<ElevatorContext>()
                    .UseSqlServer(_connectionString).Options;
                using var db = new ElevatorContext(options);
                db.ElevatorEvents.Add(ev);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Logger] Failed to log event: " + ex.Message);
            }
        }

        public async Task LogStatusAsync(ElevatorStatus status)
        {
            try
            {
                var options = new DbContextOptionsBuilder<ElevatorContext>()
                    .UseSqlServer(_connectionString).Options;
                using var db = new ElevatorContext(options);
                db.ElevatorStatuses.Add(status);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Logger] Failed to log status: " + ex.Message);
            }
        }
    }
}
