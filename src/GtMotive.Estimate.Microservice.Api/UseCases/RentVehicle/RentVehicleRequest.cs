using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>
    /// Request to rent a vehicle. The rental starts at the moment of the request.
    /// </summary>
    public sealed class RentVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Gets or sets the identifier of the vehicle. It comes from the route, not from the body.
        /// </summary>
        [JsonIgnore]
        public Guid VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the person who rents the vehicle (e.g. identity document number).
        /// A person cannot have more than one rented vehicle at the same time.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the date agreed for the return of the vehicle (ISO 8601, e.g. 2026-11-20T10:00:00Z). It must be in the future.
        /// </summary>
        [Required]
        public DateTimeOffset? EndDate { get; set; }
    }
}
