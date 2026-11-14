using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IVehicleRepository"/>.
    /// </summary>
    internal sealed class VehicleRepository(MongoService mongoService) : IVehicleRepository
    {
        private readonly IMongoCollection<Vehicle> _vehicles = mongoService.Vehicles;

        public async Task Add(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            await mongoService.EnsureIndexes();

            try
            {
                await _vehicles.InsertOneAsync(vehicle);
            }
            catch (MongoWriteException exception) when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // Two simultaneous requests with the same license plate: the unique index rejects the second one.
                throw new LicensePlateAlreadyExistsException(
                    $"There is already a vehicle with the license plate {vehicle.LicensePlate} in the fleet.",
                    exception);
            }
        }

        public async Task<Vehicle> GetById(VehicleId vehicleId)
        {
            var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, vehicleId);

            return await _vehicles.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsWithLicensePlate(LicensePlate licensePlate)
        {
            var filter = Builders<Vehicle>.Filter.Eq(v => v.LicensePlate, licensePlate);

            return await _vehicles.Find(filter).AnyAsync();
        }

        public async Task<IReadOnlyCollection<Vehicle>> GetAvailable(DateOnly oldestManufactureDate)
        {
            // A vehicle is rented while it has a rental without return date.
            var activeRentals = Builders<Rental>.Filter.Eq(r => r.ReturnDate, null);

            var rentedVehicleIds = await mongoService.Rentals
                .Find(activeRentals)
                .Project(r => r.VehicleId)
                .ToListAsync();

            var available = Builders<Vehicle>.Filter.And(
                Builders<Vehicle>.Filter.Gte(v => v.ManufactureDate, oldestManufactureDate),
                Builders<Vehicle>.Filter.Nin(v => v.Id, rentedVehicleIds));

            return await _vehicles
                .Find(available)
                .SortBy(v => v.LicensePlate)
                .ToListAsync();
        }
    }
}
