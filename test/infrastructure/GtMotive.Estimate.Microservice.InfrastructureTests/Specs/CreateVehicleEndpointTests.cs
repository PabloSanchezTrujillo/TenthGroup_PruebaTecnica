using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs
{
    /// <summary>
    /// Host-level tests of POST /api/vehicles: reception of the call and validation of the model.
    /// The use case is replaced by a spy, so no business rule nor database takes part.
    /// </summary>
    public sealed class CreateVehicleEndpointTests : InfrastructureTestBase
    {
        private static readonly Uri Endpoint = new("/api/vehicles", UriKind.Relative);

        private readonly CreateVehicleUseCaseSpy _useCase;

        public CreateVehicleEndpointTests(GenericInfrastructureTestServerFixture fixture)
            : base(fixture)
        {
            _useCase = Fixture.Server.Services.GetRequiredService<CreateVehicleUseCaseSpy>();
            _useCase.Reset();
        }

        [Fact]
        public async Task PostShouldReturnCreatedAndReachTheUseCaseWhenTheModelIsValid()
        {
            const string body = /*lang=json,strict*/ """
                { "licensePlate": " 1234 abc ", "brand": "Toyota", "model": "Corolla", "manufactureDate": "2024-05-20" }
                """;

            using var response = await Post(body);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var input = _useCase.ReceivedInputs.Should().ContainSingle().Subject;
            input.LicensePlate.ToString().Should().Be("1234 ABC");
            input.Brand.Should().Be("Toyota");
            input.Model.Should().Be("Corolla");
            input.ManufactureDate.Should().Be(new DateOnly(2024, 5, 20));

            using var json = await ReadJson(response);
            json.RootElement.GetProperty("vehicleId").GetGuid().Should().NotBeEmpty();
            json.RootElement.GetProperty("licensePlate").GetString().Should().Be("1234 ABC");
            json.RootElement.GetProperty("brand").GetString().Should().Be("Toyota");
            json.RootElement.GetProperty("model").GetString().Should().Be("Corolla");
            json.RootElement.GetProperty("manufactureDate").GetString().Should().Be("2024-05-20");
        }

        [Fact]
        public async Task PostShouldReturnBadRequestListingEveryMissingFieldWhenTheBodyIsEmpty()
        {
            using var response = await Post("{}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _useCase.ReceivedInputs.Should().BeEmpty();

            using var json = await ReadJson(response);
            var invalidFields = json.RootElement.GetProperty("errors").EnumerateObject().Select(e => e.Name);
            invalidFields.Should().BeEquivalentTo("LicensePlate", "Brand", "Model", "ManufactureDate");
        }

        [Theory]
        [InlineData(/*lang=json,strict*/ """{ "brand": "Toyota", "model": "Corolla", "manufactureDate": "2024-05-20" }""", "LicensePlate")]
        [InlineData(/*lang=json,strict*/ """{ "licensePlate": "1234 ABC", "model": "Corolla", "manufactureDate": "2024-05-20" }""", "Brand")]
        [InlineData(/*lang=json,strict*/ """{ "licensePlate": "1234 ABC", "brand": "Toyota", "manufactureDate": "2024-05-20" }""", "Model")]
        [InlineData(/*lang=json,strict*/ """{ "licensePlate": "1234 ABC", "brand": "Toyota", "model": "Corolla" }""", "ManufactureDate")]
        [InlineData(/*lang=json,strict*/ """{ "licensePlate": "1234567890123456", "brand": "Toyota", "model": "Corolla", "manufactureDate": "2024-05-20" }""", "LicensePlate")]
        public async Task PostShouldReturnBadRequestWithoutReachingTheUseCaseWhenAFieldIsInvalid(string body, string invalidField)
        {
            using var response = await Post(body);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _useCase.ReceivedInputs.Should().BeEmpty();

            using var json = await ReadJson(response);
            json.RootElement.GetProperty("errors").EnumerateObject().Select(e => e.Name).Should().Contain(invalidField);
        }

        [Theory]
        [InlineData(/*lang=json,strict*/ """{ "licensePlate": "1234 ABC", "brand": "Toyota", "model": "Corolla", "manufactureDate": "not-a-date" }""")]
        [InlineData("this is not json")]
        public async Task PostShouldReturnBadRequestWithoutReachingTheUseCaseWhenTheBodyCannotBeRead(string body)
        {
            using var response = await Post(body);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _useCase.ReceivedInputs.Should().BeEmpty();
        }

        private static async Task<JsonDocument> ReadJson(HttpResponseMessage response)
        {
            return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        }

        private async Task<HttpResponseMessage> Post(string body)
        {
            using var client = Fixture.Server.CreateClient();
            using var content = new StringContent(body, Encoding.UTF8, "application/json");

            return await client.PostAsync(Endpoint, content);
        }
    }
}
