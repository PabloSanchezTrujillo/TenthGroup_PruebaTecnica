using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases
{
    public sealed class CreateVehicleUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 13, 10, 0, 0, TimeSpan.Zero);
        private static readonly DateOnly Today = DateOnly.FromDateTime(Now.UtcDateTime);

        private readonly Mock<IVehicleRepository> _vehicleRepository = new();
        private readonly Mock<ICreateVehicleOutputPort> _outputPort = new();
        private readonly Mock<TimeProvider> _timeProvider = new();
        private readonly CreateVehicleUseCase _useCase;

        public CreateVehicleUseCaseTests()
        {
            _timeProvider.Setup(t => t.GetUtcNow()).Returns(Now);

            _useCase = new CreateVehicleUseCase(
                new VehicleService(_vehicleRepository.Object),
                _outputPort.Object,
                _timeProvider.Object);
        }

        [Fact]
        public async Task ExecuteShouldAddTheVehicleAndWriteTheOutputWhenTheVehicleIsValid()
        {
            var input = new CreateVehicleInput(new LicensePlate(" 1234 abc "), "Toyota", "Corolla", Today.AddYears(-2));
            CreateVehicleOutput output = null;
            _outputPort
                .Setup(p => p.StandardHandle(It.IsAny<CreateVehicleOutput>()))
                .Callback<CreateVehicleOutput>(o => output = o);

            await _useCase.Execute(input);

            _vehicleRepository.Verify(r => r.Add(It.Is<Vehicle>(v => v.LicensePlate == input.LicensePlate)), Times.Once);
            output.Should().NotBeNull();
            output.VehicleId.Should().NotBeEmpty();
            output.LicensePlate.Should().Be("1234 ABC");
            output.Brand.Should().Be("Toyota");
            output.Model.Should().Be("Corolla");
            output.ManufactureDate.Should().Be(Today.AddYears(-2));
        }

        [Fact]
        public async Task ExecuteShouldRejectTheVehicleWhenTheLicensePlateAlreadyExists()
        {
            var input = new CreateVehicleInput(new LicensePlate("1234 ABC"), "Toyota", "Corolla", Today.AddYears(-2));
            _vehicleRepository.Setup(r => r.ExistsWithLicensePlate(input.LicensePlate)).ReturnsAsync(true);

            var act = () => _useCase.Execute(input);

            await act.Should().ThrowAsync<LicensePlateAlreadyExistsException>();
            _vehicleRepository.Verify(r => r.Add(It.IsAny<Vehicle>()), Times.Never);
            _outputPort.Verify(p => p.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteShouldAcceptTheVehicleWhenItIsExactlyFiveYearsOld()
        {
            var input = new CreateVehicleInput(new LicensePlate("1234 ABC"), "Toyota", "Corolla", Today.AddYears(-5));

            await _useCase.Execute(input);

            _vehicleRepository.Verify(r => r.Add(It.IsAny<Vehicle>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteShouldRejectTheVehicleWhenItIsOlderThanFiveYears()
        {
            var input = new CreateVehicleInput(new LicensePlate("1234 ABC"), "Toyota", "Corolla", Today.AddYears(-5).AddDays(-1));

            var act = () => _useCase.Execute(input);

            await act.Should().ThrowAsync<VehicleTooOldException>();
            _vehicleRepository.Verify(r => r.Add(It.IsAny<Vehicle>()), Times.Never);
            _outputPort.Verify(p => p.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Never);
        }
    }
}
