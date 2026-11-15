using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Output message of the Create Vehicle use case.
    /// </summary>
    public sealed class CreateVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVehicleOutput"/> class.
        /// </summary>
        /// <param name="vehicle">Vehicle added to the fleet.</param>
        public CreateVehicleOutput(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            VehicleId = vehicle.Id.Value;
            LicensePlate = vehicle.LicensePlate.ToString();
            Brand = vehicle.Brand;
            Model = vehicle.Model;
            ManufactureDate = vehicle.ManufactureDate;
        }

        /// <summary>
        /// Gets the identifier of the new vehicle.
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
