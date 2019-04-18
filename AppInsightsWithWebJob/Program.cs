using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppInsightsWithWebJob
{
    using System.Threading;

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
            var cancellationToken = new WebJobsShutdownWatcher().Token;

//            var telemetryClient = host.Services.GetService<TelemetryClient>();
//            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) => telemetryClient.TrackException((Exception)eventArgs.ExceptionObject);
//            TaskScheduler.UnobservedTaskException += (sender, eventArgs) => telemetryClient.TrackException((Exception)eventArgs.Exception);

            using (host)
            {
                await host.RunAsync(default(CancellationToken));
            }
        }
    }
}
