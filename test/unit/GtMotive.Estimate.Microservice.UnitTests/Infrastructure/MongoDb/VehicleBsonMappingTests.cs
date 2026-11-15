using System;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Infrastructure.MongoDb
{
    public sealed class VehicleBsonMappingTests
    {
        private static readonly DateOnly Today = new(2026, 11, 13);

        public VehicleBsonMappingTests()
        {
            BsonClassMapRegistration.RegisterBsonClasses();
        }

        [Fact]
        public void VehicleShouldBeStoredWithItsValueObjectsAsPlainValues()
        {
            var vehicle = Vehicle.Create(new LicensePlate("1234 ABC"), "Toyota", "Corolla", new DateOnly(2024, 5, 20), Today);

            var document = vehicle.ToBsonDocument();

            document["_id"].AsString.Should().Be(vehicle.Id.ToString());
            document["LicensePlate"].AsString.Should().Be("1234 ABC");
            document["Brand"].AsString.Should().Be("Toyota");
            document["Model"].AsString.Should().Be("Corolla");
            document["ManufactureDate"].ToUniversalTime().Should().Be(new DateTime(2024, 5, 20, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void VehicleShouldBeReadBackFromItsDocumentEvenWhenItIsOlderThanFiveYears()
        {
            var id = Guid.NewGuid();
            var document = new BsonDocument
            {
                { "_id", id.ToString() },
                { "LicensePlate", "9876 XYZ" },
                { "Brand", "Seat" },
                { "Model", "Ibiza" },
                { "ManufactureDate", new DateTime(2015, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
            };

            var vehicle = BsonSerializer.Deserialize<Vehicle>(document);

            vehicle.Id.Should().Be(new VehicleId(id));
            vehicle.LicensePlate.Should().Be(new LicensePlate("9876 XYZ"));
            vehicle.Brand.Should().Be("Seat");
            vehicle.Model.Should().Be("Ibiza");
            vehicle.ManufactureDate.Should().Be(new DateOnly(2015, 1, 10));
            vehicle.IsWithinFleetAge(Today).Should().BeFalse();
        }
    }
}
