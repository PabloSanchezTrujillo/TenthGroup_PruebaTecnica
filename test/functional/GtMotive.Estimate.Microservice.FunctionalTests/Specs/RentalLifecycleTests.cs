using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    /// <summary>
    /// Integration tests without the host: the requests go straight to the MediatR handlers, and from there
    /// through the real use cases, domain and MongoDB repositories. No HTTP takes part.
    /// They need the MongoDB of docker-compose running and use their own database.
    /// </summary>
    public sealed class RentalLifecycleTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

        [Fact]
        public async Task RentingAVehicleShouldRemoveItFromTheAvailableVehiclesUntilItIsReturned()
        {
            var rented = await CreateVehicle("1111 AAA");
            var other = await CreateVehicle("2222 BBB");

            (await AvailableVehicleIds()).Should().BeEquivalentTo([rented, other]);

            var rentResult = await Rent(rented, "12345678Z");

            var rental = rentResult.Should().BeOfType<ObjectResult>().Which.Value.Should().BeOfType<RentalResponse>().Subject;
            rental.VehicleId.Should().Be(rented);
            rental.CustomerId.Should().Be("12345678Z");
            (await AvailableVehicleIds()).Should().BeEquivalentTo([other]);

            var returnResult = await Send(new ReturnVehicleRequest(rented));

            var returned = returnResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<ReturnVehicleResponse>().Subject;
            returned.RentalId.Should().Be(rental.RentalId);
            returned.DaysOverdue.Should().Be(0);
            returned.SurchargeNotice.Should().BeNull();
            (await AvailableVehicleIds()).Should().BeEquivalentTo([rented, other]);
        }

        [Fact]
        public async Task ACustomerShouldNotRentASecondVehicleUntilTheFirstOneIsReturned()
        {
            var first = await CreateVehicle("1111 AAA");
            var second = await CreateVehicle("2222 BBB");
            await Rent(first, "12345678Z");

            var rentingASecondOne = () => Rent(second, " 12345678z ");

            await rentingASecondOne.Should().ThrowAsync<CustomerAlreadyHasActiveRentalException>();
            (await AvailableVehicleIds()).Should().BeEquivalentTo([second]);

            await Send(new ReturnVehicleRequest(first));

            var rentResult = await Rent(second, "12345678Z");
            rentResult.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task ARentedVehicleShouldNotBeRentedByAnotherCustomer()
        {
            var vehicle = await CreateVehicle("1111 AAA");
            await Rent(vehicle, "12345678Z");

            var rentingItAgain = () => Rent(vehicle, "87654321X");

            await rentingItAgain.Should().ThrowAsync<VehicleNotAvailableException>();
        }

        [Fact]
        public async Task TheFleetShouldNotAcceptTwoVehiclesWithTheSameLicensePlate()
        {
            await CreateVehicle("1111 AAA");

            var creatingADuplicate = () => CreateVehicle(" 1111 aaa ");

            await creatingADuplicate.Should().ThrowAsync<LicensePlateAlreadyExistsException>();
            (await AvailableVehicleIds()).Should().ContainSingle();
        }

        [Fact]
        public async Task AVehicleThatHasExceededFiveYearsShouldNeitherBeListedNorRented()
        {
            // It was added to the fleet six years ago, when it was new. Today it is too old.
            var sixYearsAgo = Today.AddYears(-6);
            var oldVehicle = Vehicle.Create(new LicensePlate("9999 OLD"), "Seat", "Panda", sixYearsAgo, sixYearsAgo);
            await Fixture.UsingRepository<IVehicleRepository>(repository => repository.Add(oldVehicle));
            var recent = await CreateVehicle("1111 AAA");

            (await AvailableVehicleIds()).Should().BeEquivalentTo([recent]);

            var rentingTheOldOne = () => Rent(oldVehicle.Id.Value, "12345678Z");

            await rentingTheOldOne.Should().ThrowAsync<VehicleTooOldException>();
        }

        [Fact]
        public async Task ReturningAVehicleLateShouldFreeItAndReportTheDaysOverdueWithASurchargeNotice()
        {
            // A rental that started three days ago and should have ended 25 hours ago: two started days overdue.
            var vehicle = await CreateVehicle("1111 AAA");
            var now = DateTime.UtcNow;
            var overdueRental = Rental.Start(new VehicleId(vehicle), new CustomerId("12345678Z"), now.AddDays(-3), now.AddHours(-25));
            await Fixture.UsingRepository<IRentalRepository>(repository => repository.Add(overdueRental));

            var returnResult = await Send(new ReturnVehicleRequest(vehicle));

            var returned = returnResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<ReturnVehicleResponse>().Subject;
            returned.DaysOverdue.Should().Be(2);
            returned.SurchargeNotice.Should().Contain("2 days");
            (await AvailableVehicleIds()).Should().BeEquivalentTo([vehicle]);
        }

        [Fact]
        public async Task RentingOrReturningAVehicleThatDoesNotExistShouldReportNotFound()
        {
            var unknownVehicle = Guid.NewGuid();

            (await Rent(unknownVehicle, "12345678Z")).Should().BeOfType<NotFoundObjectResult>();
            (await Send(new ReturnVehicleRequest(unknownVehicle))).Should().BeOfType<NotFoundObjectResult>();
        }

        private async Task<Guid> CreateVehicle(string licensePlate)
        {
            var result = await Send(new CreateVehicleRequest
            {
                LicensePlate = licensePlate,
                Brand = "Toyota",
                Model = "Corolla",
                ManufactureDate = Today.AddYears(-1),
            });

            return result.Should().BeOfType<ObjectResult>().Which.Value.Should().BeOfType<VehicleResponse>().Subject.VehicleId;
        }

        private Task<IActionResult> Rent(Guid vehicleId, string customerId)
        {
            return Send(new RentVehicleRequest
            {
                VehicleId = vehicleId,
                CustomerId = customerId,
                EndDate = DateTimeOffset.UtcNow.AddDays(7),
            });
        }

        private async Task<IEnumerable<Guid>> AvailableVehicleIds()
        {
            var result = await Send(new GetAvailableVehiclesRequest());

            var vehicles = result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeAssignableTo<IEnumerable<VehicleResponse>>().Subject;

            return vehicles.Select(v => v.VehicleId);
        }

        /// <summary>
        /// Runs the handler of a request in its own scope, as a real HTTP request would do, and returns what the presenter built.
        /// </summary>
        private async Task<IActionResult> Send<TRequest>(TRequest request)
            where TRequest : IRequest<IWebApiPresenter>
        {
            IActionResult result = null;

            await Fixture.UsingHandlerForRequestResponse<TRequest, IWebApiPresenter>(async handler =>
            {
                var presenter = await handler.Handle(request, CancellationToken.None);
                result = presenter.ActionResult;
            });

            return result;
        }
    }
}
