using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElevatorSystem.Models;

namespace ElevatorSystem.Services
{
    public class ElevatorController
    {
        private readonly List<Elevator> _elevators = new();
        private readonly ElevatorScheduler _scheduler;
        private readonly ElevatorLogger _logger;
        private readonly CancellationTokenSource _cts = new();

        public ElevatorController(string connectionString)
        {
            for (int i = 1; i <= 4; i++)
                _elevators.Add(new Elevator(i, 1));

            _scheduler = new ElevatorScheduler(_elevators);
            _logger = new ElevatorLogger(connectionString);
        }

        public async Task StartAsync()
        {
            var tasks = new List<Task>();

            // start processing loops
            foreach (var e in _elevators)
            {
                tasks.Add(Task.Run(() => e.ProcessRequestsAsync(OnElevatorArriveAsync, _cts.Token)));
            }

            // generator
            tasks.Add(Task.Run(() => RequestGeneratorLoop(_cts.Token)));

            // periodic snapshot logger
            tasks.Add(Task.Run(() => PeriodicStatusLoggingLoop(_cts.Token)));

            Console.WriteLine("Press Ctrl+C to stop.");
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Stopping...");
                _cts.Cancel();
                e.Cancel = true;
            };

            await Task.WhenAll(tasks);
        }

        private async Task OnElevatorArriveAsync(Elevator elevator, int floor)
        {
            var evt = new ElevatorEvent
            {
                ElevatorId = elevator.Id,
                EventType = "Arrive",
                Description = $"Elevator {elevator.Id} at floor {floor} (Dir={elevator.Direction})",
                Timestamp = DateTime.UtcNow
            };
            Console.WriteLine($"[Event] {evt.Timestamp:o} {evt.Description}");
            await _logger.LogEventAsync(evt);

            var status = new ElevatorStatus
            {
                ElevatorId = elevator.Id,
                CurrentFloor = elevator.CurrentFloor,
                Direction = elevator.Direction,
                OccupiedTargets = elevator.PendingCount,
                Timestamp = DateTime.UtcNow
            };
            await _logger.LogStatusAsync(status);
        }

        private async Task RequestGeneratorLoop(CancellationToken ct)
        {
            var rnd = new Random();
            while (!ct.IsCancellationRequested)
            {
                int floor = rnd.Next(1, 11);
                var dir = rnd.Next(0,2) == 0 ? ElevatorDirection.Up : ElevatorDirection.Down;
                var call = new FloorCall { Floor = floor, Direction = dir, RequestedAt = DateTime.UtcNow };
                Console.WriteLine($"[Request] {call.RequestedAt:o} Floor {call.Floor} ({call.Direction})");

                var assigned = _scheduler.Assign(call);
                Console.WriteLine($"[Assign] Elevator {assigned.Id} -> Floor {call.Floor} (pending {assigned.PendingCount})");

                await Task.Delay(TimeSpan.FromSeconds(20), ct);
            }
        }

        private async Task PeriodicStatusLoggingLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                foreach (var e in _elevators)
                {
                    var status = new ElevatorStatus
                    {
                        ElevatorId = e.Id,
                        CurrentFloor = e.CurrentFloor,
                        Direction = e.Direction,
                        OccupiedTargets = e.PendingCount,
                        Timestamp = DateTime.UtcNow
                    };
                    await _logger.LogStatusAsync(status);
                }
                await Task.Delay(TimeSpan.FromSeconds(30), ct);
            }
        }
    }
}
