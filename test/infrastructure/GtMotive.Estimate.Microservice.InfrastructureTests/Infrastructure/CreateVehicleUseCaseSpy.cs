using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure
{
    /// <summary>
    /// Replaces the Create Vehicle use case in the test server. It records the inputs that reach the
    /// application layer and answers with the same data, without business rules nor database.
    /// </summary>
    public sealed class CreateVehicleUseCaseSpy
    {
        private readonly ConcurrentQueue<CreateVehicleInput> _receivedInputs = new();

        /// <summary>
        /// Gets the inputs the host has sent to the use case since the last <see cref="Reset"/>.
        /// </summary>
        public IReadOnlyCollection<CreateVehicleInput> ReceivedInputs => _receivedInputs;

        /// <summary>
        /// Forgets the recorded inputs.
        /// </summary>
        public void Reset()
        {
            _receivedInputs.Clear();
        }

        internal sealed class UseCase(CreateVehicleUseCaseSpy spy, ICreateVehicleOutputPort outputPort) : IUseCase<CreateVehicleInput>
        {
            public Task Execute(CreateVehicleInput input)
            {
                ArgumentNullException.ThrowIfNull(input);

                spy._receivedInputs.Enqueue(input);

                // "Today" is the manufacture date, so the age rule of the domain never rejects the vehicle here.
                var vehicle = Vehicle.Create(input.LicensePlate, input.Brand, input.Model, input.ManufactureDate, input.ManufactureDate);

                outputPort.StandardHandle(new CreateVehicleOutput(vehicle));

                return Task.CompletedTask;
            }
        }
    }
}
