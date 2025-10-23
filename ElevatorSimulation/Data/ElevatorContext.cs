using Microsoft.EntityFrameworkCore;
using ElevatorSystem.Models;

namespace ElevatorSystem.Data
{
    public class ElevatorContext : DbContext
    {
        public ElevatorContext(DbContextOptions<ElevatorContext> options) : base(options) { }

        public DbSet<ElevatorEvent> ElevatorEvents { get; set; } = null!;
        public DbSet<ElevatorStatus> ElevatorStatuses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ElevatorEvent>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.EventType).HasMaxLength(100);
                b.Property(e => e.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<ElevatorStatus>(b =>
            {
                b.HasKey(s => s.Id);
            });
        }
    }
}
