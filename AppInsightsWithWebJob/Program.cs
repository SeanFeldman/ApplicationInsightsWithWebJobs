namespace AppInsightsWithWebJob
{
    using System;
    using Microsoft.ApplicationInsights;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder();

            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
            });

            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
                b.AddApplicationInsights(options =>
                {
                    options.InstrumentationKey = "x";
                });
            });

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<ITelemetryInitializer, MyTelemetryInitializer>();
                services.AddSingleton<IHostedService, ContinuousJob>();
            });

            var host = builder.Build();

            var telemetryClient = host.Services.GetService<TelemetryClient>();

            try
            {
                await host.RunAsync();
            }
            catch (Exception exception)
            {
                telemetryClient.TrackException(exception);
            }
        }
    }
}
