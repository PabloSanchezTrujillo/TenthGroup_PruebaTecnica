using System;
using System.Collections.Generic;
using System.Linq;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles
{
    /// <summary>
    /// Output message of the Get Available Vehicles use case.
    /// </summary>
    public sealed class GetAvailableVehiclesOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailableVehiclesOutput"/> class.
        /// </summary>
        /// <param name="vehicles">Available vehicles.</param>
        public GetAvailableVehiclesOutput(IEnumerable<Vehicle> vehicles)
        {
            ArgumentNullException.ThrowIfNull(vehicles);

            var items = vehicles.Select(vehicle => new AvailableVehicleOutput(vehicle)).ToList();

            Vehicles = items;
        }

        /// <summary>
        /// Gets the vehicles of the fleet that can be rented.
        /// </summary>
        public IReadOnlyCollection<AvailableVehicleOutput> Vehicles { get; }
    }
}
