using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    /// <summary>
    /// Builds the input message, runs the Create Vehicle use case and returns its presenter.
    /// </summary>
    public sealed class CreateVehicleRequestHandler(
        IUseCase<CreateVehicleInput> useCase,
        CreateVehiclePresenter presenter) : IRequestHandler<CreateVehicleRequest, IWebApiPresenter>
    {
        /// <inheritdoc/>
        public async Task<IWebApiPresenter> Handle(CreateVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var input = new CreateVehicleInput(
                new LicensePlate(request.LicensePlate),
                request.Brand,
                request.Model,
                request.ManufactureDate.GetValueOrDefault());

            await useCase.Execute(input);

            return presenter;
        }
    }
}
