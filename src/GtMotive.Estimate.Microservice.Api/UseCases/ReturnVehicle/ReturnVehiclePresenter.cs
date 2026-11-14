using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    /// <summary>
    /// Turns the output of the Return Vehicle use case into an HTTP response.
    /// </summary>
    public sealed class ReturnVehiclePresenter : IWebApiPresenter, IReturnVehicleOutputPort
    {
        /// <inheritdoc/>
        public IActionResult ActionResult { get; private set; }

        /// <inheritdoc/>
        public void StandardHandle(ReturnVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            var returnResponse = new ReturnVehicleResponse
            {
                RentalId = response.RentalId,
                VehicleId = response.VehicleId,
                CustomerId = response.CustomerId,
                StartDate = response.StartDate,
                EndDate = response.EndDate,
                ReturnDate = response.ReturnDate,
                DaysOverdue = response.DaysOverdue,
                SurchargeNotice = response.HasSurcharge ? BuildSurchargeNotice(response.DaysOverdue) : null,
            };

            ActionResult = new OkObjectResult(returnResponse);
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

        private static string BuildSurchargeNotice(int daysOverdue)
        {
            var days = daysOverdue == 1 ? "1 day" : $"{daysOverdue} days";

            return $"The vehicle was returned {days} after the agreed end date. A surcharge will be applied for {days}.";
        }
    }
}
