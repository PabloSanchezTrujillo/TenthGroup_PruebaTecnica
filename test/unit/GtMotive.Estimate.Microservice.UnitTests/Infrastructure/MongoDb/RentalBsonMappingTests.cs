using System;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Infrastructure.MongoDb
{
    public sealed class RentalBsonMappingTests
    {
        private static readonly DateTime Start = new(2026, 11, 13, 10, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime End = new(2026, 11, 20, 10, 0, 0, DateTimeKind.Utc);

        public RentalBsonMappingTests()
        {
            BsonClassMapRegistration.RegisterBsonClasses();
        }

        [Fact]
        public void ActiveRentalShouldBeStoredWithItsValueObjectsAsPlainValuesAndNoReturnDate()
        {
            var vehicleId = VehicleId.New();
            var rental = Rental.Start(vehicleId, new CustomerId("12345678z"), Start, End);

            var document = rental.ToBsonDocument();

            document["_id"].AsString.Should().Be(rental.Id.ToString());
            document["VehicleId"].AsString.Should().Be(vehicleId.ToString());
            document["CustomerId"].AsString.Should().Be("12345678Z");
            document["StartDate"].ToUniversalTime().Should().Be(Start);
            document["EndDate"].ToUniversalTime().Should().Be(End);
            document["ReturnDate"].IsBsonNull.Should().BeTrue();
            document.Contains("IsActive").Should().BeFalse();
            document.Contains("DaysOverdue").Should().BeFalse();
        }

        [Fact]
        public void ReturnedRentalShouldBeReadBackFromItsDocument()
        {
            var rental = Rental.Start(VehicleId.New(), new CustomerId("12345678Z"), Start, End);
            rental.Complete(End.AddDays(2));

            var restored = BsonSerializer.Deserialize<Rental>(rental.ToBsonDocument());

            restored.Id.Should().Be(rental.Id);
            restored.VehicleId.Should().Be(rental.VehicleId);
            restored.CustomerId.Should().Be(rental.CustomerId);
            restored.StartDate.Should().Be(Start);
            restored.EndDate.Should().Be(End);
            restored.ReturnDate.Should().Be(End.AddDays(2));
            restored.IsActive.Should().BeFalse();
            restored.DaysOverdue.Should().Be(2);
        }
    }
}
