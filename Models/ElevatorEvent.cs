using System;

namespace ElevatorSystem.Models
{
    public class ElevatorEvent
    {
        public int Id { get; set; }
        public int ElevatorId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
