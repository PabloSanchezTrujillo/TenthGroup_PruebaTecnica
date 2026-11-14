using System;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Api.UseCases
{
    public sealed class ReturnVehiclePresenterTests
    {
        private static readonly DateTime Start = new(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime End = new(2026, 11, 8, 10, 0, 0, DateTimeKind.Utc);

        private readonly ReturnVehiclePresenter _presenter = new();

        [Fact]
        public void StandardHandleShouldNotIncludeASurchargeNoticeWhenTheVehicleIsReturnedOnTime()
        {
            _presenter.StandardHandle(ReturnedAt(End));

            var response = Response();
            response.DaysOverdue.Should().Be(0);
            response.SurchargeNotice.Should().BeNull();
        }

        [Fact]
        public void StandardHandleShouldWarnAboutTheSurchargeForOneDayWhenTheVehicleIsReturnedOneHourLate()
        {
            _presenter.StandardHandle(ReturnedAt(End.AddHours(1)));

            var response = Response();
            response.DaysOverdue.Should().Be(1);
            response.SurchargeNotice.Should().Be(
                "The vehicle was returned 1 day after the agreed end date. A surcharge will be applied for 1 day.");
        }

        [Fact]
        public void StandardHandleShouldWarnAboutTheSurchargeWithTheDaysOverdueWhenTheVehicleIsReturnedSeveralDaysLate()
        {
            _presenter.StandardHandle(ReturnedAt(End.AddHours(49)));

            var response = Response();
            response.DaysOverdue.Should().Be(3);
            response.SurchargeNotice.Should().Be(
                "The vehicle was returned 3 days after the agreed end date. A surcharge will be applied for 3 days.");
        }

        [Fact]
        public void NotFoundHandleShouldProduceANotFoundResultWithTheMessage()
        {
            _presenter.NotFoundHandle("The vehicle is not currently rented.");

            var result = _presenter.ActionResult.Should().BeOfType<NotFoundObjectResult>().Subject;
            result.Value.Should().BeOfType<ProblemDetails>().Which.Detail.Should().Be("The vehicle is not currently rented.");
        }

        private static ReturnVehicleOutput ReturnedAt(DateTime returnDate)
        {
            var rental = Rental.Start(VehicleId.New(), new CustomerId("12345678Z"), Start, End);
            rental.Complete(returnDate);

            return new ReturnVehicleOutput(rental);
        }

        private ReturnVehicleResponse Response()
        {
            var result = _presenter.ActionResult.Should().BeOfType<OkObjectResult>().Subject;

            return result.Value.Should().BeOfType<ReturnVehicleResponse>().Subject;
        }
    }
}
