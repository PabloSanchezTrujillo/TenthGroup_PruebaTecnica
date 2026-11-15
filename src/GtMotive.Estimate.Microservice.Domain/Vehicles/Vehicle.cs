using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// A vehicle of the rental fleet.
    /// </summary>
    public sealed class Vehicle
    {
        /// <summary>
        /// Maximum age, in years since its manufacture date, a vehicle can have to be part of the fleet.
        /// </summary>
        public const int VehicleMaxAge = 5;

        private Vehicle(VehicleId id, LicensePlate licensePlate, string brand, string model, DateOnly manufactureDate)
        {
            Id = id;
            LicensePlate = licensePlate;
            Brand = brand;
            Model = model;
            ManufactureDate = manufactureDate;
        }

        /// <summary>
        /// Gets the identity of the vehicle.
        /// </summary>
        public VehicleId Id { get; private set; }

        /// <summary>
        /// Gets the license plate.
        /// </summary>
        public LicensePlate LicensePlate { get; private set; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        public string Brand { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        public DateOnly ManufactureDate { get; private set; }

        /// <summary>
        /// Creates a new vehicle for the fleet.
        /// </summary>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="today">Current date, used to check the age of the vehicle.</param>
        /// <returns>The new <see cref="Vehicle"/>.</returns>
        /// <exception cref="VehicleTooOldException">The vehicle is older than <see cref="VehicleMaxAge"/> years.</exception>
        public static Vehicle Create(LicensePlate licensePlate, string brand, string model, DateOnly manufactureDate, DateOnly today)
        {
            ArgumentNullException.ThrowIfNull(licensePlate);

            if (string.IsNullOrWhiteSpace(brand))
            {
                throw new DomainException("The brand is required.");
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                throw new DomainException("The model is required.");
            }

            if (manufactureDate > today)
            {
                throw new DomainException("The manufacture date cannot be in the future.");
            }

            if (manufactureDate < today.AddYears(-VehicleMaxAge))
            {
                throw new VehicleTooOldException(
                    $"The fleet does not accept vehicles manufactured more than {VehicleMaxAge} years ago.");
            }

            return new Vehicle(VehicleId.New(), licensePlate, brand.Trim(), model.Trim(), manufactureDate);
        }

        /// <summary>
        /// Checks if the vehicle is still within the age allowed in the fleet.
        /// </summary>
        /// <param name="today">Current date.</param>
        /// <returns>True if the vehicle is not older than <see cref="VehicleMaxAge"/> years.</returns>
        public bool IsWithinFleetAge(DateOnly today)
        {
            return ManufactureDate >= today.AddYears(-VehicleMaxAge);
        }
    }
}
