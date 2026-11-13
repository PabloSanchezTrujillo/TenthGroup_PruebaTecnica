using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// The rental of a vehicle by a customer.
    /// It is active from its start until the vehicle is returned.
    /// </summary>
    public sealed class Rental
    {
        private Rental(RentalId id, VehicleId vehicleId, CustomerId customerId, DateTime startDate, DateTime endDate)
        {
            Id = id;
            VehicleId = vehicleId;
            CustomerId = customerId;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Gets the identity of the rental.
        /// </summary>
        public RentalId Id { get; private set; }

        /// <summary>
        /// Gets the identity of the rented vehicle.
        /// </summary>
        public VehicleId VehicleId { get; private set; }

        /// <summary>
        /// Gets the identity of the customer who rents the vehicle.
        /// </summary>
        public CustomerId CustomerId { get; private set; }

        /// <summary>
        /// Gets the date the rental started.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Gets the date agreed for the return of the vehicle.
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Gets the date the vehicle was actually returned, or null while the rental is active.
        /// </summary>
        public DateTime? ReturnDate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the vehicle has not been returned yet,
        /// regardless of the agreed <see cref="EndDate"/>.
        /// </summary>
        public bool IsActive => ReturnDate is null;

        /// <summary>
        /// Gets the number of days the vehicle was returned after the agreed <see cref="EndDate"/>.
        /// Any started day counts as a whole day. It is zero while the rental is active
        /// or when the vehicle was returned on time.
        /// </summary>
        public int DaysOverdue
        {
            get
            {
                if (ReturnDate is null || ReturnDate.Value <= EndDate)
                {
                    return 0;
                }

                return (int)Math.Ceiling((ReturnDate.Value - EndDate).TotalDays);
            }
        }

        /// <summary>
        /// Starts a new rental.
        /// </summary>
        /// <param name="vehicleId">Identity of the rented vehicle.</param>
        /// <param name="customerId">Identity of the customer.</param>
        /// <param name="startDate">Date the rental starts.</param>
        /// <param name="endDate">Date agreed for the return of the vehicle.</param>
        /// <returns>The new active <see cref="Rental"/>.</returns>
        public static Rental Start(VehicleId vehicleId, CustomerId customerId, DateTime startDate, DateTime endDate)
        {
            ArgumentNullException.ThrowIfNull(vehicleId);
            ArgumentNullException.ThrowIfNull(customerId);

            if (endDate <= startDate)
            {
                throw new DomainException("The end date of the rental must be later than its start date.");
            }

            return new Rental(RentalId.New(), vehicleId, customerId, startDate, endDate);
        }

        /// <summary>
        /// Ends the rental because the vehicle has been returned.
        /// A late return is accepted: check <see cref="DaysOverdue"/> to know if a surcharge applies.
        /// </summary>
        /// <param name="returnDate">Date the vehicle is returned.</param>
        public void Complete(DateTime returnDate)
        {
            if (!IsActive)
            {
                throw new DomainException("The vehicle of this rental has already been returned.");
            }

            if (returnDate < StartDate)
            {
                throw new DomainException("The return date cannot be earlier than the start date of the rental.");
            }

            ReturnDate = returnDate;
        }
    }
}
