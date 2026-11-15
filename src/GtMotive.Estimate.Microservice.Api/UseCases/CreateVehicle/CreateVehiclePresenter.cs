using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    /// <summary>
    /// Turns the output of the Create Vehicle use case into an HTTP response.
    /// </summary>
    public sealed class CreateVehiclePresenter : IWebApiPresenter, ICreateVehicleOutputPort
    {
        /// <inheritdoc/>
        public IActionResult ActionResult { get; private set; }

        /// <inheritdoc/>
        public void StandardHandle(CreateVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            var vehicleResponse = new VehicleResponse(
                response.VehicleId,
                response.LicensePlate,
                response.Brand,
                response.Model,
                response.ManufactureDate);

            ActionResult = new ObjectResult(vehicleResponse)
            {
                StatusCode = StatusCodes.Status201Created,
            };
        }
    }
}
