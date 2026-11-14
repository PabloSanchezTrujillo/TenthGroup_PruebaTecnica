using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IRentalRepository"/>.
    /// </summary>
    internal sealed class RentalRepository(MongoService mongoService) : IRentalRepository
    {
        private readonly IMongoCollection<Rental> _rentals = mongoService.Rentals;

        public async Task Add(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            await mongoService.EnsureIndexes();

            try
            {
                await _rentals.InsertOneAsync(rental);
            }
            catch (MongoWriteException exception) when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // Two simultaneous requests passed the checks of the domain: the unique indexes reject the second one.
                if (exception.WriteError.Message.Contains(MongoService.ActiveRentalByCustomerIndexName, StringComparison.Ordinal))
                {
                    throw new CustomerAlreadyHasActiveRentalException(
                        $"The customer {rental.CustomerId} already has a rented vehicle and cannot rent another one at the same time.",
                        exception);
                }

                throw new VehicleNotAvailableException($"The vehicle {rental.VehicleId} is already rented.", exception);
            }
        }

        public async Task Update(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            var filter = Builders<Rental>.Filter.Eq(r => r.Id, rental.Id);

            await _rentals.ReplaceOneAsync(filter, rental);
        }

        public async Task<Rental> GetActiveByVehicle(VehicleId vehicleId)
        {
            var filter = Builders<Rental>.Filter.And(
                Builders<Rental>.Filter.Eq(r => r.VehicleId, vehicleId),
                Builders<Rental>.Filter.Eq(r => r.ReturnDate, null));

            return await _rentals.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Rental> GetActiveByCustomer(CustomerId customerId)
        {
            var filter = Builders<Rental>.Filter.And(
                Builders<Rental>.Filter.Eq(r => r.CustomerId, customerId),
                Builders<Rental>.Filter.Eq(r => r.ReturnDate, null));

            return await _rentals.Find(filter).FirstOrDefaultAsync();
        }
    }
}
