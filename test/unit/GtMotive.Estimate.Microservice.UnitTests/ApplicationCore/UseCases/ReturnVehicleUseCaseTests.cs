using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases
{
    public sealed class ReturnVehicleUseCaseTests
    {
        private static readonly DateTime Start = new(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime End = new(2026, 11, 8, 10, 0, 0, DateTimeKind.Utc);

        private readonly Mock<IRentalRepository> _rentalRepository = new();
        private readonly Mock<IReturnVehicleOutputPort> _outputPort = new();
        private readonly Mock<TimeProvider> _timeProvider = new();
        private readonly ReturnVehicleUseCase _useCase;

        private ReturnVehicleOutput _output;

        public ReturnVehicleUseCaseTests()
        {
            _outputPort
                .Setup(p => p.StandardHandle(It.IsAny<ReturnVehicleOutput>()))
                .Callback<ReturnVehicleOutput>(o => _output = o);

            _useCase = new ReturnVehicleUseCase(_rentalRepository.Object, _outputPort.Object, _timeProvider.Object);
        }

        [Fact]
        public async Task ExecuteShouldCompleteTheRentalWithoutSurchargeWhenTheVehicleIsReturnedOnTime()
        {
            var rental = ActiveRental();
            ReturningAt(End.AddHours(-3));

            await _useCase.Execute(new ReturnVehicleInput(rental.VehicleId));

            _rentalRepository.Verify(r => r.Update(It.Is<Rental>(x => x.Id == rental.Id && !x.IsActive)), Times.Once);
            _output.RentalId.Should().Be(rental.Id.Value);
            _output.VehicleId.Should().Be(rental.VehicleId.Value);
            _output.CustomerId.Should().Be("12345678Z");
            _output.ReturnDate.Should().Be(End.AddHours(-3));
            _output.DaysOverdue.Should().Be(0);
            _output.HasSurcharge.Should().BeFalse();
        }

        [Fact]
        public async Task ExecuteShouldNotApplySurchargeWhenTheVehicleIsReturnedExactlyAtTheEndDate()
        {
            var rental = ActiveRental();
            ReturningAt(End);

            await _useCase.Execute(new ReturnVehicleInput(rental.VehicleId));

            _output.DaysOverdue.Should().Be(0);
            _output.HasSurcharge.Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(24, 1)]
        [InlineData(25, 2)]
        [InlineData(72, 3)]
        public async Task ExecuteShouldCountAnyStartedDayAsAWholeDayOverdue(int hoursLate, int expectedDaysOverdue)
        {
            var rental = ActiveRental();
            ReturningAt(End.AddHours(hoursLate));

            await _useCase.Execute(new ReturnVehicleInput(rental.VehicleId));

            _rentalRepository.Verify(r => r.Update(It.Is<Rental>(x => !x.IsActive)), Times.Once);
            _output.DaysOverdue.Should().Be(expectedDaysOverdue);
            _output.HasSurcharge.Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteShouldReportNotFoundWhenTheVehicleHasNoActiveRental()
        {
            ReturningAt(End);

            await _useCase.Execute(new ReturnVehicleInput(VehicleId.New()));

            _outputPort.Verify(p => p.NotFoundHandle(It.IsAny<string>()), Times.Once);
            _outputPort.Verify(p => p.StandardHandle(It.IsAny<ReturnVehicleOutput>()), Times.Never);
            _rentalRepository.Verify(r => r.Update(It.IsAny<Rental>()), Times.Never);
        }

        private Rental ActiveRental()
        {
            var rental = Rental.Start(VehicleId.New(), new CustomerId("12345678Z"), Start, End);
            _rentalRepository.Setup(r => r.GetActiveByVehicle(rental.VehicleId)).ReturnsAsync(rental);

            return rental;
        }

        private void ReturningAt(DateTime utcNow)
        {
            _timeProvider.Setup(t => t.GetUtcNow()).Returns(new DateTimeOffset(utcNow));
        }
    }
}
