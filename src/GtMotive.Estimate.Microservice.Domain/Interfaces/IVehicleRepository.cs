using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Repository of the <see cref="Vehicle"/> aggregate.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Adds a new vehicle to the fleet.
        /// </summary>
        /// <param name="vehicle">Vehicle to add.</param>
        /// <returns>Task.</returns>
        Task Add(Vehicle vehicle);

        /// <summary>
        /// Gets a vehicle by its identity.
        /// </summary>
        /// <param name="vehicleId">Identity of the vehicle.</param>
        /// <returns>The vehicle, or null if it does not exist.</returns>
        Task<Vehicle> GetById(VehicleId vehicleId);

        /// <summary>
        /// Checks if there is already a vehicle with a license plate in the fleet.
        /// </summary>
        /// <param name="licensePlate">License plate to look for.</param>
        /// <returns>True if a vehicle with that license plate exists.</returns>
        Task<bool> ExistsWithLicensePlate(LicensePlate licensePlate);

        /// <summary>
        /// Gets the vehicles of the fleet that are not rented.
        /// </summary>
        /// <param name="oldestManufactureDate">Vehicles manufactured before this date are no longer part of the fleet.</param>
        /// <returns>The available vehicles.</returns>
        Task<IReadOnlyCollection<Vehicle>> GetAvailable(DateOnly oldestManufactureDate);
    }
}
