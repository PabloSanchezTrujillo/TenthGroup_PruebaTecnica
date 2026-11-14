using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>
    /// Turns the output of the Rent Vehicle use case into an HTTP response.
    /// </summary>
    public sealed class RentVehiclePresenter : IWebApiPresenter, IRentVehicleOutputPort
    {
        /// <inheritdoc/>
        public IActionResult ActionResult { get; private set; }

        /// <inheritdoc/>
        public void StandardHandle(RentVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            var rentalResponse = new RentalResponse(
                response.RentalId,
                response.VehicleId,
                response.CustomerId,
                response.StartDate,
                response.EndDate);

            ActionResult = new ObjectResult(rentalResponse)
            {
                StatusCode = StatusCodes.Status201Created,
            };
        }

        /// <inheritdoc/>
        public void NotFoundHandle(string message)
        {
            ActionResult = new NotFoundObjectResult(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = message,
            });
        }
    }
}
