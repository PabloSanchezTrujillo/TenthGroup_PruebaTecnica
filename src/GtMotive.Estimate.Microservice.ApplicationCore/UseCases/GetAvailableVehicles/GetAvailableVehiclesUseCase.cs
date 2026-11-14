using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles
{
    /// <summary>
    /// Use case that lists the vehicles of the fleet that can be rented.
    /// </summary>
    public sealed class GetAvailableVehiclesUseCase(
        IVehicleRepository vehicleRepository,
        IGetAvailableVehiclesOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<GetAvailableVehiclesInput>
    {
        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Input message. It carries no data.</param>
        /// <returns>Task.</returns>
        public async Task Execute(GetAvailableVehiclesInput input)
        {
            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

            // Vehicles that have exceeded the maximum age are no longer part of the fleet.
            var oldestManufactureDate = today.AddYears(-Vehicle.VehicleMaxAge);

            var vehicles = await vehicleRepository.GetAvailable(oldestManufactureDate);

            outputPort.StandardHandle(new GetAvailableVehiclesOutput(vehicles));
        }
    }
}
