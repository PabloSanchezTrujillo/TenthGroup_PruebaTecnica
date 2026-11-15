using System;

namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// Identity of a <see cref="Rental"/>.
    /// </summary>
    public sealed record RentalId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentalId"/> class.
        /// </summary>
        /// <param name="value">Underlying identifier.</param>
        public RentalId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException("The rental id is required.");
            }

            Value = value;
        }

        /// <summary>
        /// Gets the underlying identifier.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Creates a new unique identity.
        /// </summary>
        /// <returns>A new <see cref="RentalId"/>.</returns>
        public static RentalId New()
        {
            return new RentalId(Guid.NewGuid());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
