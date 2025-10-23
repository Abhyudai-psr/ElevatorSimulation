using Xunit;
using Xunit.Abstractions;
using ElevatorSystem.Models;
using ElevatorSystem.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorSimulation.Tests
{
    public class ElevatorTests
    {
        private readonly ITestOutputHelper _output;

        public ElevatorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Elevator should reach its target floor")]
        public async Task Elevator_ShouldReachTargetFloor()
        {
            // Arrange
            _output.WriteLine("Starting test: Elevator should reach its target floor.");

            var elevator = new Elevator(1);
            elevator.AddRequest(3);
            _output.WriteLine("Elevator initialized at floor 1 and given a request to move to floor 3.");

            int finalFloor = 0;
            async Task OnArrive(Elevator e, int floor)
            {
                _output.WriteLine($"Elevator arrived at floor {floor}. Direction: {e.Direction}");
                finalFloor = floor;
                await Task.CompletedTask;
            }

            var cts = new CancellationTokenSource();
            var processTask = elevator.ProcessRequestsAsync(OnArrive, cts.Token);

            // Act
            _output.WriteLine("Simulating elevator movement for approximately 35 seconds...");
            await Task.Delay(35000);
            cts.Cancel();

            // Assert
            _output.WriteLine($"Elevator final floor: {finalFloor}");
            Assert.Equal(3, finalFloor);
            _output.WriteLine("Test completed successfully: Elevator reached the expected floor.");
        }

        [Fact(DisplayName = "Scheduler should pick the nearest elevator")]
        public void Scheduler_ShouldPickNearestElevator()
        {
            // Arrange
            _output.WriteLine("Starting test: Scheduler should pick the nearest elevator.");

            var elevators = new List<Elevator>
            {
                new Elevator(1),
                new Elevator(2),
                new Elevator(3)
            };

            // Manually set current floors
            typeof(Elevator).GetProperty("CurrentFloor")!.SetValue(elevators[0], 2);
            typeof(Elevator).GetProperty("CurrentFloor")!.SetValue(elevators[1], 7);
            typeof(Elevator).GetProperty("CurrentFloor")!.SetValue(elevators[2], 5);

            _output.WriteLine("Elevator 1 at floor 2");
            _output.WriteLine("Elevator 2 at floor 7");
            _output.WriteLine("Elevator 3 at floor 5");

            var scheduler = new ElevatorScheduler(elevators);
            var call = new FloorCall { Floor = 4, Direction = ElevatorDirection.Up };
            _output.WriteLine("Created a floor call: Floor 4 (Up).");

            // Act
            var chosen = scheduler.Assign(call);
            _output.WriteLine($"Scheduler assigned Elevator {chosen.Id} to handle the request.");

            // Assert
            Assert.Equal(3, chosen.Id);
            _output.WriteLine("Test completed successfully: Scheduler selected the nearest elevator.");
        }

        [Fact(DisplayName = "Elevator direction should update correctly when moving up")]
        public async Task Elevator_Direction_ShouldBeUp()
        {
            // Arrange
            _output.WriteLine("Starting test: Elevator direction should update correctly when moving up.");

            var elevator = new Elevator(1);
            elevator.AddRequest(4);
            _output.WriteLine("Elevator initialized at floor 1 and given a request to move to floor 4.");

            ElevatorDirection currentDirection = ElevatorDirection.Idle;
            async Task OnArrive(Elevator e, int floor)
            {
                currentDirection = e.Direction;
                _output.WriteLine($"Elevator arrived at floor {floor}. Current direction: {e.Direction}");
                await Task.CompletedTask;
            }

            var cts = new CancellationTokenSource();
            var task = elevator.ProcessRequestsAsync(OnArrive, cts.Token);

            // Act
            _output.WriteLine("Waiting 15 seconds to allow elevator to start moving...");
            await Task.Delay(15000);
            cts.Cancel();

            // Assert
            _output.WriteLine($"Observed elevator direction: {currentDirection}");
            Assert.Equal(ElevatorDirection.Up, currentDirection);
            _output.WriteLine("Test completed successfully: Elevator direction updated as expected.");
        }
    }
}
