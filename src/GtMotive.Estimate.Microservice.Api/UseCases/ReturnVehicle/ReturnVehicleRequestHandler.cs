using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    /// <summary>
    /// Builds the input message, runs the Return Vehicle use case and returns its presenter.
    /// </summary>
    public sealed class ReturnVehicleRequestHandler(
        IUseCase<ReturnVehicleInput> useCase,
        ReturnVehiclePresenter presenter) : IRequestHandler<ReturnVehicleRequest, IWebApiPresenter>
    {
        /// <inheritdoc/>
        public async Task<IWebApiPresenter> Handle(ReturnVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            await useCase.Execute(new ReturnVehicleInput(new VehicleId(request.VehicleId)));

            return presenter;
        }
    }
}
