using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Serializers
{
    /// <summary>
    /// Stores a <see cref="LicensePlate"/> as a string.
    /// </summary>
    internal sealed class LicensePlateSerializer : SerializerBase<LicensePlate>
    {
        public override LicensePlate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new LicensePlate(context.Reader.ReadString());
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, LicensePlate value)
        {
            context.Writer.WriteString(value.ToString());
        }
    }
}
