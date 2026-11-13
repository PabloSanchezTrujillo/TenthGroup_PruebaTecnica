using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// Holds the rental rules that involve more than one aggregate.
    /// </summary>
    public sealed class RentalService(IRentalRepository rentalRepository)
    {
        private readonly IRentalRepository _rentalRepository = rentalRepository;

        /// <summary>
        /// Rents a vehicle to a customer.
        /// </summary>
        /// <param name="vehicle">Vehicle to rent.</param>
        /// <param name="customerId">Identity of the customer.</param>
        /// <param name="startDate">Date the rental starts.</param>
        /// <param name="endDate">Date agreed for the return of the vehicle.</param>
        /// <returns>The new active <see cref="Rental"/>.</returns>
        /// <exception cref="VehicleTooOldException">The vehicle is no longer within the age allowed in the fleet.</exception>
        /// <exception cref="VehicleNotAvailableException">The vehicle is already rented.</exception>
        /// <exception cref="CustomerAlreadyHasActiveRentalException">The customer already has a rented vehicle.</exception>
        public async Task<Rental> Rent(Vehicle vehicle, CustomerId customerId, DateTime startDate, DateTime endDate)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            ArgumentNullException.ThrowIfNull(customerId);

            if (!vehicle.IsWithinFleetAge(DateOnly.FromDateTime(startDate)))
            {
                throw new VehicleTooOldException(
                    $"The vehicle {vehicle.Id} is no longer part of the fleet because it was manufactured more than {Vehicle.VehicleMaxAge} years ago.");
            }

            if (await _rentalRepository.GetActiveByVehicle(vehicle.Id) is not null)
            {
                throw new VehicleNotAvailableException($"The vehicle {vehicle.Id} is already rented.");
            }

            if (await _rentalRepository.GetActiveByCustomer(customerId) is not null)
            {
                throw new CustomerAlreadyHasActiveRentalException(
                    $"The customer {customerId} already has a rented vehicle and cannot rent another one at the same time.");
            }

            var rental = Rental.Start(vehicle.Id, customerId, startDate, endDate);

            await _rentalRepository.Add(rental);

            return rental;
        }
    }
}
