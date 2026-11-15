using System;
using GtMotive.Estimate.Microservice.Domain.Rentals;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Output message of the Rent Vehicle use case.
    /// </summary>
    public sealed class RentVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleOutput"/> class.
        /// </summary>
        /// <param name="rental">Rental that has just started.</param>
        public RentVehicleOutput(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            RentalId = rental.Id.Value;
            VehicleId = rental.VehicleId.Value;
            CustomerId = rental.CustomerId.ToString();
            StartDate = rental.StartDate;
            EndDate = rental.EndDate;
        }

        /// <summary>
        /// Gets the identifier of the rental.
        /// </summary>
        public Guid RentalId { get; }

        /// <summary>
        /// Gets the identifier of the rented vehicle.
        /// </summary>
        public Guid VehicleId { get; }

        /// <summary>
        /// Gets the identifier of the customer.
        /// </summary>
        public string CustomerId { get; }

        /// <summary>
        /// Gets the date the rental started (UTC).
        /// </summary>
        public DateTime StartDate { get; }

        /// <summary>
        /// Gets the date agreed for the return of the vehicle (UTC).
        /// </summary>
        public DateTime EndDate { get; }
    }
}
