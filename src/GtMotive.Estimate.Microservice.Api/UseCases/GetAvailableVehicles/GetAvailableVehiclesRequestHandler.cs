using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles
{
    /// <summary>
    /// Runs the Get Available Vehicles use case and returns its presenter.
    /// </summary>
    public sealed class GetAvailableVehiclesRequestHandler(
        IUseCase<GetAvailableVehiclesInput> useCase,
        GetAvailableVehiclesPresenter presenter) : IRequestHandler<GetAvailableVehiclesRequest, IWebApiPresenter>
    {
        /// <inheritdoc/>
        public async Task<IWebApiPresenter> Handle(GetAvailableVehiclesRequest request, CancellationToken cancellationToken)
        {
            await useCase.Execute(new GetAvailableVehiclesInput());

            return presenter;
        }
    }
}
