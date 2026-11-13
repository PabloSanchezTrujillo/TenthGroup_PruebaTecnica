using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Serializers;
using MongoDB.Bson.Serialization;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    /// <summary>
    /// Tells the MongoDB driver how to store the domain classes, so the domain does not need to know about MongoDB.
    /// </summary>
    public static class BsonClassMapRegistration
    {
        /// <summary>
        /// Registers the serializers and class maps of the domain. It can be called more than once.
        /// </summary>
        public static void RegisterBsonClasses()
        {
            BsonSerializer.TryRegisterSerializer(new VehicleIdSerializer());
            BsonSerializer.TryRegisterSerializer(new LicensePlateSerializer());

            BsonClassMap.TryRegisterClassMap<Vehicle>(map =>
            {
                map.MapIdProperty(v => v.Id);
                map.MapProperty(v => v.LicensePlate);
                map.MapProperty(v => v.Brand);
                map.MapProperty(v => v.Model);
                map.MapProperty(v => v.ManufactureDate);
            });
        }
    }
}
