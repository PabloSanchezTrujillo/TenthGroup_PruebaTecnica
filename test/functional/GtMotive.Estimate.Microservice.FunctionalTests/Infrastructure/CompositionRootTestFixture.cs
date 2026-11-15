using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Infrastructure;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Xunit;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    public sealed class CompositionRootTestFixture : IDisposable, IAsyncLifetime
    {
        private const string TestDatabaseSuffix = "_tests";

        private readonly ServiceProvider _serviceProvider;

        public CompositionRootTestFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            Configuration = configuration;
            ConfigureServices(services, configuration);
            services.AddSingleton<IConfiguration>(configuration);
            _serviceProvider = services.BuildServiceProvider();
        }

        public IConfiguration Configuration { get; }

        public async Task InitializeAsync()
        {
            await ResetDatabase();
        }

        public async Task DisposeAsync()
        {
            var database = GetTestDatabase();

            await database.Client.DropDatabaseAsync(database.DatabaseNamespace.DatabaseName);
        }

        /// <summary>
        /// Empties the collections of the test database, keeping their indexes.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task ResetDatabase()
        {
            GetTestDatabase();

            var mongoService = _serviceProvider.GetRequiredService<MongoService>();

            await mongoService.Vehicles.DeleteManyAsync(FilterDefinition<Vehicle>.Empty);
            await mongoService.Rentals.DeleteManyAsync(FilterDefinition<Rental>.Empty);
        }

        public async Task UsingHandlerForRequest<TRequest>(Func<IRequestHandler<TRequest, Unit>, Task> handlerAction)
            where TRequest : IRequest<Unit>
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, Unit>>();

            await handlerAction.Invoke(handler);
        }

        public async Task UsingHandlerForRequestResponse<TRequest, TResponse>(Func<IRequestHandler<TRequest, TResponse>, Task> handlerAction)
            where TRequest : IRequest<TResponse>
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            if (handler == null)
            {
                Debug.Fail("The requested handler has not been registered");
            }

            await handlerAction.Invoke(handler);
        }

        public async Task UsingRepository<TRepository>(Func<TRepository, Task> handlerAction)
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TRepository>();

            if (handler == null)
            {
                Debug.Fail("The requested handler has not been registered");
            }

            await handlerAction.Invoke(handler);
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
            services.AddApiDependencies();
            services.AddLogging();
            services.AddBaseInfrastructure(true);
        }

        /// <summary>
        /// Safety net: these tests delete data, so they refuse to run against a database that is not a test one.
        /// </summary>
        private IMongoDatabase GetTestDatabase()
        {
            var database = _serviceProvider.GetRequiredService<MongoService>().Database;
            var name = database.DatabaseNamespace.DatabaseName;

            return name.EndsWith(TestDatabaseSuffix, StringComparison.Ordinal)
                ? database
                : throw new InvalidOperationException(
                    $"The functional tests delete data and only run against a database whose name ends with '{TestDatabaseSuffix}', but 'MongoDb:MongoDbDatabaseName' is '{name}'.");
        }
    }
}
