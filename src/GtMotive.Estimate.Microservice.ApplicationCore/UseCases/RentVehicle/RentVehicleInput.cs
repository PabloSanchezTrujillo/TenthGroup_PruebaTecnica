using System;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Input message of the Rent Vehicle use case.
    /// </summary>
    public sealed class RentVehicleInput(VehicleId vehicleId, CustomerId customerId, DateTime endDate) : IUseCaseInput
    {
        /// <summary>
        /// Gets the identity of the vehicle to rent.
        /// </summary>
        public VehicleId VehicleId { get; } = vehicleId;

        /// <summary>
        /// Gets the identity of the customer who rents the vehicle.
        /// </summary>
        public CustomerId CustomerId { get; } = customerId;

        /// <summary>
        /// Gets the date agreed for the return of the vehicle (UTC).
        /// </summary>
        public DateTime EndDate { get; } = endDate;
    }
}
