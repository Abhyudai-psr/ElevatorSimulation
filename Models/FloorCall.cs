using System;

namespace ElevatorSystem.Models
{
    public class FloorCall
    {
        public int Floor { get; set; }
        public ElevatorDirection Direction { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
