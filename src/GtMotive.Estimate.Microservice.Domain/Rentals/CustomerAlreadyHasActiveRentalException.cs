using System;
using System.Diagnostics.CodeAnalysis;

namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// Thrown when a customer who already has a rented vehicle tries to rent another one.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CustomerAlreadyHasActiveRentalException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAlreadyHasActiveRentalException"/> class.
        /// </summary>
        public CustomerAlreadyHasActiveRentalException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAlreadyHasActiveRentalException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public CustomerAlreadyHasActiveRentalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAlreadyHasActiveRentalException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CustomerAlreadyHasActiveRentalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
