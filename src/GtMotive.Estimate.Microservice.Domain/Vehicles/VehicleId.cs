using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value Object. Identity of a <see cref="Vehicle"/>.
    /// </summary>
    public sealed record VehicleId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleId"/> class.
        /// </summary>
        /// <param name="value">Underlying identifier.</param>
        public VehicleId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException("The vehicle id is required.");
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
        /// <returns>A new <see cref="VehicleId"/>.</returns>
        public static VehicleId New()
        {
            return new VehicleId(Guid.NewGuid());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
