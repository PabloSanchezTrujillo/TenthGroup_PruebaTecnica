using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    public class MongoService
    {
        private const string VehiclesCollectionName = "vehicles";

        private Task _indexes;

        public MongoService(IOptions<MongoDbSettings> options)
        {
            BsonClassMapRegistration.RegisterBsonClasses();

            MongoClient = new MongoClient(options.Value.ConnectionString);
            Database = MongoClient.GetDatabase(options.Value.MongoDbDatabaseName);
        }

        public MongoClient MongoClient { get; }

        public IMongoDatabase Database { get; }

        public IMongoCollection<Vehicle> Vehicles => Database.GetCollection<Vehicle>(VehiclesCollectionName);

        /// <summary>
        /// Creates the indexes the first time it is called. If it fails (e.g. the database is down) the next call retries.
        /// </summary>
        /// <returns>Task.</returns>
        public Task EnsureIndexes()
        {
            if (_indexes is null || _indexes.IsFaulted || _indexes.IsCanceled)
            {
                _indexes = CreateIndexes();
            }

            return _indexes;
        }

        private async Task CreateIndexes()
        {
            // Second guard of the "unique license plate" rule, for simultaneous requests.
            var uniqueLicensePlate = new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.LicensePlate),
                new CreateIndexOptions { Unique = true, Name = "ux_vehicles_licensePlate" });

            await Vehicles.Indexes.CreateOneAsync(uniqueLicensePlate);
        }
    }
}
