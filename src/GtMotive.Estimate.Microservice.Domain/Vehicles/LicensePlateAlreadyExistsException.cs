using System;
using System.Diagnostics.CodeAnalysis;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Thrown when trying to add to the fleet a vehicle whose license plate is already registered.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LicensePlateAlreadyExistsException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyExistsException"/> class.
        /// </summary>
        public LicensePlateAlreadyExistsException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public LicensePlateAlreadyExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public LicensePlateAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
