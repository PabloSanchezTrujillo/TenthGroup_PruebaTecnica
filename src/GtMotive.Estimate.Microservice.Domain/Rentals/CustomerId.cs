namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// Identifies the person who rents a vehicle (e.g. the number of an identity document),
    /// stored trimmed and in upper case.
    /// </summary>
    public sealed record CustomerId
    {
        /// <summary>
        /// Maximum number of characters allowed.
        /// </summary>
        public const int MaxLength = 50;

        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerId"/> class.
        /// </summary>
        /// <param name="value">Identifier of the person.</param>
        public CustomerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("The customer id is required.");
            }

            var normalized = value.Trim().ToUpperInvariant();

            if (normalized.Length > MaxLength)
            {
                throw new DomainException($"The customer id cannot exceed {MaxLength} characters.");
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
