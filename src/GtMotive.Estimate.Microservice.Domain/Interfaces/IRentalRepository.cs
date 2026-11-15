using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Repository of the <see cref="Rental"/> aggregate.
    /// </summary>
    public interface IRentalRepository
    {
        /// <summary>
        /// Adds a new rental.
        /// </summary>
        /// <param name="rental">Rental to add.</param>
        /// <returns>Task.</returns>
        Task Add(Rental rental);

        /// <summary>
        /// Updates an existing rental.
        /// </summary>
        /// <param name="rental">Rental to update.</param>
        /// <returns>Task.</returns>
        Task Update(Rental rental);

        /// <summary>
        /// Gets the active rental of a vehicle.
        /// </summary>
        /// <param name="vehicleId">Identity of the vehicle.</param>
        /// <returns>The active rental, or null if the vehicle is not rented.</returns>
        Task<Rental> GetActiveByVehicle(VehicleId vehicleId);

        /// <summary>
        /// Gets the active rental of a customer.
        /// </summary>
        /// <param name="customerId">Identity of the customer.</param>
        /// <returns>The active rental, or null if the customer has no rented vehicle.</returns>
        Task<Rental> GetActiveByCustomer(CustomerId customerId);
    }
}
