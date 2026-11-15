using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>
    /// Builds the input message, runs the Rent Vehicle use case and returns its presenter.
    /// </summary>
    public sealed class RentVehicleRequestHandler(
        IUseCase<RentVehicleInput> useCase,
        RentVehiclePresenter presenter) : IRequestHandler<RentVehicleRequest, IWebApiPresenter>
    {
        /// <inheritdoc/>
        public async Task<IWebApiPresenter> Handle(RentVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var input = new RentVehicleInput(
                new VehicleId(request.VehicleId),
                new CustomerId(request.CustomerId),
                request.EndDate.GetValueOrDefault().UtcDateTime);

            await useCase.Execute(input);

            return presenter;
        }
    }
}
