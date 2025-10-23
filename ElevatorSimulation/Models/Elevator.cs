using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorSystem.Models
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; private set; } = 1;
        public ElevatorDirection Direction { get; private set; } = ElevatorDirection.Idle;

        // simple queue for target floors
        private readonly ConcurrentQueue<int> _requests = new();

        public Elevator(int id, int initialFloor = 1)
        {
            Id = id;
            CurrentFloor = initialFloor;
        }

        public int PendingCount => _requests.Count;

        public void AddRequest(int floor)
        {
            _requests.Enqueue(floor);
        }

        public async Task ProcessRequestsAsync(Func<Elevator, int, Task> onArriveAsync, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_requests.TryDequeue(out var target))
                {
                    Direction = target > CurrentFloor ? ElevatorDirection.Up :
                                target < CurrentFloor ? ElevatorDirection.Down :
                                ElevatorDirection.Idle;

                    // move floor by floor
                    while (CurrentFloor != target && !ct.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10), ct);
                        CurrentFloor += Direction == ElevatorDirection.Up ? 1 : -1;
                        await onArriveAsync(this, CurrentFloor);
                    }

                    // arrived at target floor
                    await onArriveAsync(this, CurrentFloor);

                    // simulate dwell time
                    await Task.Delay(TimeSpan.FromSeconds(10), ct);

                    // if no more requests, go idle
                    Direction = _requests.IsEmpty ? ElevatorDirection.Idle : Direction;
                }
                else
                {
                    Direction = ElevatorDirection.Idle;
                    await Task.Delay(500, ct);
                }
            }
        }
    }
}
