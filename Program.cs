using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ElevatorSystem.Services;
using Microsoft.EntityFrameworkCore;
using ElevatorSystem.Data;

namespace ElevatorSystem
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== ElevatorSimulation v1.0 ===");

            // load configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var conn = config.GetConnectionString("ElevatorDatabase") 
                       ?? "Server=.;Database=ElevatorDB;Trusted_Connection=True;TrustServerCertificate=True;";

            // Try to create DB/tables if possible (EnsureCreated)
            var options = new DbContextOptionsBuilder<ElevatorContext>()
                .UseSqlServer(conn)
                .Options;

            try
            {
                using var db = new ElevatorContext(options);
                db.Database.EnsureCreated();
                Console.WriteLine("Database ensured/created (if connection valid).");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: DB ensure/create failed. Check connection string. " + ex.Message);
            }

            var controller = new ElevatorController(conn);
            await controller.StartAsync();
            Console.WriteLine("Simulation ended.");
        }
    }
}
