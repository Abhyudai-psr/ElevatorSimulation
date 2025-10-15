using System;

namespace ElevatorSystem.Models
{
    public class ElevatorStatus
    {
        public int Id { get; set; }
        public int ElevatorId { get; set; }
        public int CurrentFloor { get; set; }
        public ElevatorDirection Direction { get; set; }
        public int OccupiedTargets { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
