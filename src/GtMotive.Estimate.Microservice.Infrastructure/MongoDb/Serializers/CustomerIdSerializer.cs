using GtMotive.Estimate.Microservice.Domain.Rentals;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Serializers
{
    /// <summary>
    /// Stores a <see cref="CustomerId"/> as a string.
    /// </summary>
    internal sealed class CustomerIdSerializer : SerializerBase<CustomerId>
    {
        public override CustomerId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new CustomerId(context.Reader.ReadString());
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CustomerId value)
        {
            context.Writer.WriteString(value.ToString());
        }
    }
}
