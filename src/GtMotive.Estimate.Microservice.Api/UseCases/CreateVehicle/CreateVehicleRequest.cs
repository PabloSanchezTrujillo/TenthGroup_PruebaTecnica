using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    /// <summary>
    /// Request to add a new vehicle to the fleet.
    /// </summary>
    public sealed class CreateVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Gets or sets the license plate. It must be unique in the fleet.
        /// </summary>
        [Required]
        [StringLength(15)]
        public string LicensePlate { get; set; }

        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        [Required]
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        [Required]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the manufacture date (yyyy-MM-dd). It cannot be more than 5 years ago.
        /// </summary>
        [Required]
        public DateOnly? ManufactureDate { get; set; }
    }
}
