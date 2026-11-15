using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Input message of the Return Vehicle use case.
    /// </summary>
    public sealed class ReturnVehicleInput(VehicleId vehicleId) : IUseCaseInput
    {
        /// <summary>
        /// Gets the identity of the vehicle that is being returned.
        /// </summary>
        public VehicleId VehicleId { get; } = vehicleId;
    }
}
