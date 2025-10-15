using System;
using System.Collections.Generic;
using System.Linq;
using ElevatorSystem.Models;

namespace ElevatorSystem.Services
{
    public class ElevatorScheduler
    {
        private readonly List<Elevator> _elevators;

        public ElevatorScheduler(List<Elevator> elevators)
        {
            _elevators = elevators;
        }

        public Elevator Assign(FloorCall call)
        {
            // Simple nearest-elevator algorithm, tie-breaker by fewest pending requests
            var chosen = _elevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - call.Floor))
                .ThenBy(e => e.PendingCount)
                .First();

            chosen.AddRequest(call.Floor);
            return chosen;
        }
    }
}
