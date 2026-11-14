using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles
{
    /// <summary>
    /// A vehicle of the fleet that can be rented.
    /// </summary>
    public sealed class AvailableVehicleOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableVehicleOutput"/> class.
        /// </summary>
        /// <param name="vehicle">Available vehicle.</param>
        public AvailableVehicleOutput(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            VehicleId = vehicle.Id.Value;
            LicensePlate = vehicle.LicensePlate.ToString();
            Brand = vehicle.Brand;
            Model = vehicle.Model;
            ManufactureDate = vehicle.ManufactureDate;
        }

        /// <summary>
        /// Gets the identifier of the vehicle.
        /// </summary>
        public Guid VehicleId { get; }

        /// <summary>
        /// Gets the license plate.
        /// </summary>
        public string LicensePlate { get; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        public string Brand { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        public DateOnly ManufactureDate { get; }
    }
}
