using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// Vehicles of the rental fleet.
    /// </summary>
    [ApiController]
    [Route("api/vehicles")]
    [Produces("application/json")]
    public sealed class VehiclesController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Adds a new vehicle to the fleet.
        /// </summary>
        /// <param name="request">Data of the vehicle.</param>
        /// <returns>The vehicle added to the fleet.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody][Required] CreateVehicleRequest request)
        {
            var presenter = await mediator.Send(request);

            return presenter.ActionResult;
        }

        /// <summary>
        /// Lists the vehicles of the fleet that can be rented.
        /// </summary>
        /// <returns>The vehicles that are not rented and are still within the age allowed in the fleet.</returns>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<VehicleResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailable()
        {
            var presenter = await mediator.Send(new GetAvailableVehiclesRequest());

            return presenter.ActionResult;
        }

        /// <summary>
        /// Rents a vehicle to a person. The rental starts now and the vehicle stops being available until it is returned.
        /// </summary>
        /// <param name="vehicleId">Identifier of the vehicle to rent.</param>
        /// <param name="request">Person who rents the vehicle and agreed end date.</param>
        /// <returns>The rental that has just started.</returns>
        [HttpPost("{vehicleId:guid}/rentals")]
        [ProducesResponseType(typeof(RentalResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Rent([FromRoute] Guid vehicleId, [FromBody][Required] RentVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            request.VehicleId = vehicleId;

            var presenter = await mediator.Send(request);

            return presenter.ActionResult;
        }

        /// <summary>
        /// Returns a rented vehicle. The return happens now and the vehicle becomes available again.
        /// If it is returned after the agreed end date, the response includes the days overdue and a surcharge notice.
        /// </summary>
        /// <param name="vehicleId">Identifier of the vehicle that is being returned.</param>
        /// <returns>The completed rental.</returns>
        [HttpPost("{vehicleId:guid}/return")]
        [ProducesResponseType(typeof(ReturnVehicleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Return([FromRoute] Guid vehicleId)
        {
            var presenter = await mediator.Send(new ReturnVehicleRequest(vehicleId));

            return presenter.ActionResult;
        }
    }
}
