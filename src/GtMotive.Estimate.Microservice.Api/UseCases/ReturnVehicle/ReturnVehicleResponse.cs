using System;
using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    /// <summary>
    /// Result of returning a rented vehicle.
    /// </summary>
    public sealed class ReturnVehicleResponse
    {
        /// <summary>
        /// Gets the identifier of the rental.
        /// </summary>
        [Required]
        public Guid RentalId { get; init; }

        /// <summary>
        /// Gets the identifier of the returned vehicle.
        /// </summary>
        [Required]
        public Guid VehicleId { get; init; }

        /// <summary>
        /// Gets the identifier of the customer.
        /// </summary>
        [Required]
        public string CustomerId { get; init; }

        /// <summary>
        /// Gets the date the rental started (UTC).
        /// </summary>
        [Required]
        public DateTime StartDate { get; init; }

        /// <summary>
        /// Gets the date agreed for the return of the vehicle (UTC).
        /// </summary>
        [Required]
        public DateTime EndDate { get; init; }

        /// <summary>
        /// Gets the date the vehicle was actually returned (UTC).
        /// </summary>
        [Required]
        public DateTime ReturnDate { get; init; }

        /// <summary>
        /// Gets the number of days the vehicle was returned after the agreed end date.
        /// Any started day counts as a whole day. It is 0 when the vehicle was returned on time.
        /// </summary>
        [Required]
        public int DaysOverdue { get; init; }

        /// <summary>
        /// Gets the notice of the surcharge for the late return, or null when the vehicle was returned on time.
        /// </summary>
        public string SurchargeNotice { get; init; }
    }
}
