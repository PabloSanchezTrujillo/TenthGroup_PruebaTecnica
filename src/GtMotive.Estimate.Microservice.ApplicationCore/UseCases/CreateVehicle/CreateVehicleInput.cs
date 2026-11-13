using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Input message of the Create Vehicle use case.
    /// </summary>
    public sealed class CreateVehicleInput(LicensePlate licensePlate, string brand, string model, DateOnly manufactureDate) : IUseCaseInput
    {
        /// <summary>
        /// Gets the license plate.
        /// </summary>
        public LicensePlate LicensePlate { get; } = licensePlate;

        /// <summary>
        /// Gets the brand.
        /// </summary>
        public string Brand { get; } = brand;

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; } = model;

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        public DateOnly ManufactureDate { get; } = manufactureDate;
    }
}
