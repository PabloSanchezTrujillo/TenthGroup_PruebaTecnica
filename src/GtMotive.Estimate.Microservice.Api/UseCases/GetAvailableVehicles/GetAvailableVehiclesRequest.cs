using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles
{
    /// <summary>
    /// Request to list the vehicles of the fleet that can be rented. It needs no data.
    /// </summary>
    public sealed class GetAvailableVehiclesRequest : IRequest<IWebApiPresenter>
    {
    }
}
