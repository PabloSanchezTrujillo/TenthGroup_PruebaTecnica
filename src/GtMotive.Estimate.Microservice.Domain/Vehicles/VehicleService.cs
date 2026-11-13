using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Domain Service. Holds the fleet rules that need to know about the other vehicles.
    /// </summary>
    public sealed class VehicleService(IVehicleRepository vehicleRepository)
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

        /// <summary>
        /// Adds a new vehicle to the fleet.
        /// </summary>
        /// <param name="licensePlate">License plate. It must be unique in the fleet.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="today">Current date, used to check the age of the vehicle.</param>
        /// <returns>The new <see cref="Vehicle"/>.</returns>
        /// <exception cref="LicensePlateAlreadyExistsException">There is already a vehicle with that license plate.</exception>
        /// <exception cref="VehicleTooOldException">The vehicle is older than <see cref="Vehicle.VehicleMaxAge"/> years.</exception>
        public async Task<Vehicle> Register(LicensePlate licensePlate, string brand, string model, DateOnly manufactureDate, DateOnly today)
        {
            ArgumentNullException.ThrowIfNull(licensePlate);

            var vehicle = Vehicle.Create(licensePlate, brand, model, manufactureDate, today);

            if (await _vehicleRepository.ExistsWithLicensePlate(licensePlate))
            {
                throw new LicensePlateAlreadyExistsException(
                    $"There is already a vehicle with the license plate {licensePlate} in the fleet.");
            }

            await _vehicleRepository.Add(vehicle);

            return vehicle;
        }
    }
}
