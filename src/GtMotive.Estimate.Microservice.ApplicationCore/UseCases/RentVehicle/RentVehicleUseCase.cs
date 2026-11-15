using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Use case that rents a vehicle of the fleet to a customer. The rental starts at the moment of the request.
    /// </summary>
    public sealed class RentVehicleUseCase(
        IVehicleRepository vehicleRepository,
        RentalService rentalService,
        IRentVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<RentVehicleInput>
    {
        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Vehicle, customer and agreed end date.</param>
        /// <returns>Task.</returns>
        /// <exception cref="VehicleTooOldException">The vehicle is no longer within the age allowed in the fleet.</exception>
        /// <exception cref="VehicleNotAvailableException">The vehicle is already rented.</exception>
        /// <exception cref="CustomerAlreadyHasActiveRentalException">The customer already has a rented vehicle.</exception>
        public async Task Execute(RentVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = await vehicleRepository.GetById(input.VehicleId);

            if (vehicle is null)
            {
                outputPort.NotFoundHandle($"The vehicle {input.VehicleId} does not exist.");
                return;
            }

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var rental = await rentalService.Rent(vehicle, input.CustomerId, now, input.EndDate);

            outputPort.StandardHandle(new RentVehicleOutput(rental));
        }
    }
}
