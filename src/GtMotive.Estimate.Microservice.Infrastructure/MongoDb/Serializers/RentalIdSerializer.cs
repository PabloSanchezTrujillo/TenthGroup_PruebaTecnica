using System;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Serializers
{
    /// <summary>
    /// Stores a <see cref="RentalId"/> as a string.
    /// </summary>
    internal sealed class RentalIdSerializer : SerializerBase<RentalId>
    {
        public override RentalId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new RentalId(Guid.Parse(context.Reader.ReadString()));
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, RentalId value)
        {
            context.Writer.WriteString(value.ToString());
        }
    }
}
