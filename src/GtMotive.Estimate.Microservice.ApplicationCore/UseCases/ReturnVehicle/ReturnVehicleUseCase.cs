using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Use case that returns a rented vehicle. The return happens at the moment of the request
    /// and the vehicle becomes available again, even if it is returned late.
    /// </summary>
    public sealed class ReturnVehicleUseCase(
        IRentalRepository rentalRepository,
        IReturnVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<ReturnVehicleInput>
    {
        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Vehicle that is being returned.</param>
        /// <returns>Task.</returns>
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var rental = await rentalRepository.GetActiveByVehicle(input.VehicleId);

            if (rental is null)
            {
                outputPort.NotFoundHandle($"The vehicle {input.VehicleId} does not exist or is not currently rented.");
                return;
            }

            rental.Complete(timeProvider.GetUtcNow().UtcDateTime);

            await rentalRepository.Update(rental);

            outputPort.StandardHandle(new ReturnVehicleOutput(rental));
        }
    }
}
