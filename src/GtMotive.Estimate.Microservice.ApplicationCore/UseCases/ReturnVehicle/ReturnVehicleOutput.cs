using System;
using GtMotive.Estimate.Microservice.Domain.Rentals;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Output message of the Return Vehicle use case.
    /// </summary>
    public sealed class ReturnVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleOutput"/> class.
        /// </summary>
        /// <param name="rental">Rental that has just been completed.</param>
        public ReturnVehicleOutput(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            RentalId = rental.Id.Value;
            VehicleId = rental.VehicleId.Value;
            CustomerId = rental.CustomerId.ToString();
            StartDate = rental.StartDate;
            EndDate = rental.EndDate;
            ReturnDate = rental.ReturnDate.GetValueOrDefault();
            DaysOverdue = rental.DaysOverdue;
        }

        /// <summary>
        /// Gets the identifier of the rental.
        /// </summary>
        public Guid RentalId { get; }

        /// <summary>
        /// Gets the identifier of the returned vehicle.
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

        /// <summary>
        /// Gets the date the vehicle was actually returned (UTC).
        /// </summary>
        public DateTime ReturnDate { get; }

        /// <summary>
        /// Gets the number of days the vehicle was returned after the agreed end date. Any started day counts as a whole day.
        /// </summary>
        public int DaysOverdue { get; }

        /// <summary>
        /// Gets a value indicating whether the vehicle was returned late, so a surcharge applies.
        /// </summary>
        public bool HasSurcharge => DaysOverdue > 0;
    }
}
