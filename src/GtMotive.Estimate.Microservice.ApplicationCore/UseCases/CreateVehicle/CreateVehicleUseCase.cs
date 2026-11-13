using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Use case that adds a new vehicle to the fleet.
    /// </summary>
    public sealed class CreateVehicleUseCase(
        VehicleService vehicleService,
        ICreateVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<CreateVehicleInput>
    {
        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Data of the vehicle to add.</param>
        /// <returns>Task.</returns>
        /// <exception cref="LicensePlateAlreadyExistsException">There is already a vehicle with that license plate.</exception>
        /// <exception cref="VehicleTooOldException">The vehicle exceeds the maximum age allowed in the fleet.</exception>
        public async Task Execute(CreateVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

            var vehicle = await vehicleService.Register(
                input.LicensePlate,
                input.Brand,
                input.Model,
                input.ManufactureDate,
                today);

            outputPort.StandardHandle(new CreateVehicleOutput(vehicle));
        }
    }
}
