using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Serializers
{
    /// <summary>
    /// Stores a <see cref="VehicleId"/> as a string.
    /// </summary>
    internal sealed class VehicleIdSerializer : SerializerBase<VehicleId>
    {
        public override VehicleId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new VehicleId(Guid.Parse(context.Reader.ReadString()));
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, VehicleId value)
        {
            context.Writer.WriteString(value.ToString());
        }
    }
}
