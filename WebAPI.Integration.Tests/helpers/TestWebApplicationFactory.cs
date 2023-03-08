using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using Xunit.Abstractions;

namespace WebAPI.Integration.Tests.helpers
{
    public class TestWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private ITestOutputHelper OutputHelper { get; }

        public TestWebApplicationFactory(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
            //Server.BaseAddress = new Uri("http://localhost:8500");
        }
        
        protected override IWebHostBuilder? CreateWebHostBuilder()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            return base.CreateWebHostBuilder();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("urls", $"http://localhost:{Program.ApplicationPort}");

            // sample to configure test services
            base.ConfigureWebHost(builder);


            builder.UseSerilog((ctx, cfg) =>
            {
                cfg
                .Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
                .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Sink(new TestOutputHelperSink(OutputHelper));
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of any resources created during the test...
            }
            base.Dispose(disposing);
        }
    }
}
