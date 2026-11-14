using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases
{
    public sealed class GetAvailableVehiclesUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 13, 10, 0, 0, TimeSpan.Zero);
        private static readonly DateOnly Today = DateOnly.FromDateTime(Now.UtcDateTime);

        private readonly Mock<IVehicleRepository> _vehicleRepository = new();
        private readonly Mock<IGetAvailableVehiclesOutputPort> _outputPort = new();
        private readonly Mock<TimeProvider> _timeProvider = new();
        private readonly GetAvailableVehiclesUseCase _useCase;

        private GetAvailableVehiclesOutput _output;

        public GetAvailableVehiclesUseCaseTests()
        {
            _timeProvider.Setup(t => t.GetUtcNow()).Returns(Now);
            _outputPort
                .Setup(p => p.StandardHandle(It.IsAny<GetAvailableVehiclesOutput>()))
                .Callback<GetAvailableVehiclesOutput>(o => _output = o);

            _useCase = new GetAvailableVehiclesUseCase(_vehicleRepository.Object, _outputPort.Object, _timeProvider.Object);
        }

        [Fact]
        public async Task ExecuteShouldWriteTheAvailableVehiclesToTheOutput()
        {
            var vehicle = Vehicle.Create(new LicensePlate("1234 ABC"), "Toyota", "Corolla", Today.AddYears(-2), Today);
            _vehicleRepository.Setup(r => r.GetAvailable(It.IsAny<DateOnly>())).ReturnsAsync([vehicle]);

            await _useCase.Execute(new GetAvailableVehiclesInput());

            _output.Vehicles.Should().ContainSingle();
            var listed = _output.Vehicles.Single();
            listed.VehicleId.Should().Be(vehicle.Id.Value);
            listed.LicensePlate.Should().Be("1234 ABC");
            listed.Brand.Should().Be("Toyota");
            listed.Model.Should().Be("Corolla");
            listed.ManufactureDate.Should().Be(Today.AddYears(-2));
        }

        [Fact]
        public async Task ExecuteShouldAskOnlyForVehiclesManufacturedInTheLastFiveYears()
        {
            _vehicleRepository.Setup(r => r.GetAvailable(It.IsAny<DateOnly>())).ReturnsAsync([]);

            await _useCase.Execute(new GetAvailableVehiclesInput());

            _vehicleRepository.Verify(r => r.GetAvailable(new DateOnly(2021, 11, 13)), Times.Once);
        }

        [Fact]
        public async Task ExecuteShouldWriteAnEmptyListWhenThereAreNoAvailableVehicles()
        {
            _vehicleRepository.Setup(r => r.GetAvailable(It.IsAny<DateOnly>())).ReturnsAsync([]);

            await _useCase.Execute(new GetAvailableVehiclesInput());

            _output.Vehicles.Should().BeEmpty();
        }
    }
}
