using Dapr;
using Dapr.Client;
using Man.Dapr.Sidekick;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WebAPI.Integration.Tests.helpers;
using Xunit.Abstractions;

namespace WebAPI.Integration.Tests
{
    public class DaprTests : IAsyncLifetime
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;

        public DaprClient DaprClient
        {
            get
            {
                return _factory.Services.GetRequiredService<DaprClient>();
            }
        }

        public DaprTests(ITestOutputHelper output)
        {
            _output = output;
            _factory = new TestWebApplicationFactory<Program>(_output);
        }

        public async Task InitializeAsync()
        {
            var daprHostProcess = _factory.Services.GetRequiredService<IDaprSidecarHost>();
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            while (!(await daprHostProcess.GetHealthAsync(cancellation.Token)).IsHealthy &&
                   !cancellation.Token.IsCancellationRequested)
            {
                await Task.Delay(1000);
                Log.Warning("Waiting on dapr sidecar");
            }

            if (cancellation.Token.IsCancellationRequested)
            {
                throw new DaprException("Timed out waiting for Dapr sidecar to start.");
            }

            Log.Information("Dapr sidecar started");
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Dapr_Should_Be_Healthy()
        {
            Assert.True(await DaprClient.CheckHealthAsync());
        }
    }

}