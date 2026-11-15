using System;
using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.UseCases
{
    /// <summary>
    /// A vehicle of the fleet.
    /// </summary>
    public sealed class VehicleResponse(Guid vehicleId, string licensePlate, string brand, string model, DateOnly manufactureDate)
    {
        /// <summary>
        /// Gets the identifier of the vehicle.
        /// </summary>
        [Required]
        public Guid VehicleId { get; } = vehicleId;

        /// <summary>
        /// Gets the license plate.
        /// </summary>
        [Required]
        public string LicensePlate { get; } = licensePlate;

        /// <summary>
        /// Gets the brand.
        /// </summary>
        [Required]
        public string Brand { get; } = brand;

        /// <summary>
        /// Gets the model.
        /// </summary>
        [Required]
        public string Model { get; } = model;

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        [Required]
        public DateOnly ManufactureDate { get; } = manufactureDate;
    }
}
