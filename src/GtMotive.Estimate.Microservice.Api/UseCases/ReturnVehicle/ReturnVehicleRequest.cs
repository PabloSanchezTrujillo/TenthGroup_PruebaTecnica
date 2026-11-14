using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    /// <summary>
    /// Request to return a rented vehicle. The return happens at the moment of the request.
    /// </summary>
    public sealed class ReturnVehicleRequest(Guid vehicleId) : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Gets the identifier of the vehicle that is being returned.
        /// </summary>
        public Guid VehicleId { get; } = vehicleId;
    }
}
