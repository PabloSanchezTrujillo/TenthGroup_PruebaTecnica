using System;
using System.Linq;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles
{
    /// <summary>
    /// Turns the output of the Get Available Vehicles use case into an HTTP response.
    /// </summary>
    public sealed class GetAvailableVehiclesPresenter : IWebApiPresenter, IGetAvailableVehiclesOutputPort
    {
        /// <inheritdoc/>
        public IActionResult ActionResult { get; private set; }

        /// <inheritdoc/>
        public void StandardHandle(GetAvailableVehiclesOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            var vehicles = response.Vehicles
                .Select(v => new VehicleResponse(v.VehicleId, v.LicensePlate, v.Brand, v.Model, v.ManufactureDate))
                .ToList();

            ActionResult = new OkObjectResult(vehicles);
        }
    }
}
