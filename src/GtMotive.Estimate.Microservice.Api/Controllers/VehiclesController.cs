using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
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
    }
}
