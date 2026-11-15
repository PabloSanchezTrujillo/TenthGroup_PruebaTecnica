using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases
{
    public sealed class RentVehicleUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 13, 10, 0, 0, TimeSpan.Zero);
        private static readonly DateTime EndDate = Now.UtcDateTime.AddDays(7);
        private static readonly CustomerId Customer = new("12345678Z");

        private readonly Mock<IVehicleRepository> _vehicleRepository = new();
        private readonly Mock<IRentalRepository> _rentalRepository = new();
        private readonly Mock<IRentVehicleOutputPort> _outputPort = new();
        private readonly Mock<TimeProvider> _timeProvider = new();
        private readonly RentVehicleUseCase _useCase;

        public RentVehicleUseCaseTests()
        {
            _timeProvider.Setup(t => t.GetUtcNow()).Returns(Now);

            _useCase = new RentVehicleUseCase(
                _vehicleRepository.Object,
                new RentalService(_rentalRepository.Object),
                _outputPort.Object,
                _timeProvider.Object);
        }

        [Fact]
        public async Task ExecuteShouldStartTheRentalNowAndWriteTheOutputWhenEverythingIsValid()
        {
            var vehicle = ExistingVehicle(manufacturedYearsAgo: 2);
            RentVehicleOutput output = null;
            _outputPort
                .Setup(p => p.StandardHandle(It.IsAny<RentVehicleOutput>()))
                .Callback<RentVehicleOutput>(o => output = o);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, Customer, EndDate));

            _rentalRepository.Verify(r => r.Add(It.Is<Rental>(x => x.VehicleId == vehicle.Id && x.IsActive)), Times.Once);
            output.Should().NotBeNull();
            output.RentalId.Should().NotBeEmpty();
            output.VehicleId.Should().Be(vehicle.Id.Value);
            output.CustomerId.Should().Be("12345678Z");
            output.StartDate.Should().Be(Now.UtcDateTime);
            output.EndDate.Should().Be(EndDate);
        }

        [Fact]
        public async Task ExecuteShouldReportNotFoundWhenTheVehicleDoesNotExist()
        {
            var input = new RentVehicleInput(VehicleId.New(), Customer, EndDate);

            await _useCase.Execute(input);

            _outputPort.Verify(p => p.NotFoundHandle(It.IsAny<string>()), Times.Once);
            _outputPort.Verify(p => p.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
            _rentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteShouldRejectTheRentalWhenTheCustomerAlreadyHasARentedVehicle()
        {
            var vehicle = ExistingVehicle(manufacturedYearsAgo: 2);
            var otherRental = Rental.Start(VehicleId.New(), Customer, Now.UtcDateTime.AddDays(-1), EndDate);
            _rentalRepository.Setup(r => r.GetActiveByCustomer(Customer)).ReturnsAsync(otherRental);

            var act = () => _useCase.Execute(new RentVehicleInput(vehicle.Id, Customer, EndDate));

            await act.Should().ThrowAsync<CustomerAlreadyHasActiveRentalException>();
            _rentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Never);
            _outputPort.Verify(p => p.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteShouldRejectTheRentalWhenTheVehicleIsAlreadyRented()
        {
            var vehicle = ExistingVehicle(manufacturedYearsAgo: 2);
            var currentRental = Rental.Start(vehicle.Id, new CustomerId("87654321X"), Now.UtcDateTime.AddDays(-1), EndDate);
            _rentalRepository.Setup(r => r.GetActiveByVehicle(vehicle.Id)).ReturnsAsync(currentRental);

            var act = () => _useCase.Execute(new RentVehicleInput(vehicle.Id, Customer, EndDate));

            await act.Should().ThrowAsync<VehicleNotAvailableException>();
            _rentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteShouldRejectTheRentalWhenTheVehicleHasExceededFiveYearsSinceItWasAdded()
        {
            var vehicle = ExistingVehicle(manufacturedYearsAgo: 6);

            var act = () => _useCase.Execute(new RentVehicleInput(vehicle.Id, Customer, EndDate));

            await act.Should().ThrowAsync<VehicleTooOldException>();
            _rentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteShouldRejectTheRentalWhenTheEndDateIsNotInTheFuture()
        {
            var vehicle = ExistingVehicle(manufacturedYearsAgo: 2);

            var act = () => _useCase.Execute(new RentVehicleInput(vehicle.Id, Customer, Now.UtcDateTime.AddHours(-1)));

            await act.Should().ThrowAsync<DomainException>();
            _rentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Never);
        }

        private Vehicle ExistingVehicle(int manufacturedYearsAgo)
        {
            // The vehicle was added to the fleet the day it was manufactured, so it was valid at that moment.
            var manufactureDate = DateOnly.FromDateTime(Now.UtcDateTime).AddYears(-manufacturedYearsAgo);
            var vehicle = Vehicle.Create(new LicensePlate("1234 ABC"), "Toyota", "Corolla", manufactureDate, manufactureDate);
            _vehicleRepository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            return vehicle;
        }
    }
}
