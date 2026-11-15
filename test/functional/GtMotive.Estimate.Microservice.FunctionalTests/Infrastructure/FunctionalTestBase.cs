using System.Threading.Tasks;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    [Collection(TestCollections.Functional)]
    public abstract class FunctionalTestBase(CompositionRootTestFixture fixture) : IAsyncLifetime
    {
        public const int QueueWaitingTimeInMilliseconds = 1000;

        protected CompositionRootTestFixture Fixture { get; } = fixture;

        /// <summary>
        /// Every test starts with an empty database, so tests do not depend on each other.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task InitializeAsync()
        {
            await Fixture.ResetDatabase();
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
