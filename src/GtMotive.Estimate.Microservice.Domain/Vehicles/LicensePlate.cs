namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value Object. License plate of a <see cref="Vehicle"/>, stored trimmed and in upper case.
    /// </summary>
    public sealed record LicensePlate
    {
        /// <summary>
        /// Maximum number of characters allowed.
        /// </summary>
        public const int MaxLength = 15;

        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlate"/> class.
        /// </summary>
        /// <param name="value">License plate text.</param>
        public LicensePlate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("The license plate is required.");
            }

            var normalized = value.Trim().ToUpperInvariant();

            if (normalized.Length > MaxLength)
            {
                throw new DomainException($"The license plate cannot exceed {MaxLength} characters.");
            }

            _value = normalized;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _value;
        }
    }
}
