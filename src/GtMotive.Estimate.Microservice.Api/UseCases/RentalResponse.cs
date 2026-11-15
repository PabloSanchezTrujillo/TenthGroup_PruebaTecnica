using System;
using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.UseCases
{
    /// <summary>
    /// The rental of a vehicle.
    /// </summary>
    public sealed class RentalResponse(Guid rentalId, Guid vehicleId, string customerId, DateTime startDate, DateTime endDate)
    {
        /// <summary>
        /// Gets the identifier of the rental.
        /// </summary>
        [Required]
        public Guid RentalId { get; } = rentalId;

        /// <summary>
        /// Gets the identifier of the rented vehicle.
        /// </summary>
        [Required]
        public Guid VehicleId { get; } = vehicleId;

        /// <summary>
        /// Gets the identifier of the customer.
        /// </summary>
        [Required]
        public string CustomerId { get; } = customerId;

        /// <summary>
        /// Gets the date the rental started (UTC).
        /// </summary>
        [Required]
        public DateTime StartDate { get; } = startDate;

        /// <summary>
        /// Gets the date agreed for the return of the vehicle (UTC).
        /// </summary>
        [Required]
        public DateTime EndDate { get; } = endDate;
    }
}
