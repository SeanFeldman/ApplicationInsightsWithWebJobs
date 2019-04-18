namespace AppInsightsWithWebJob
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Microsoft.ApplicationInsights;

    public class ContinuousJob : BackgroundService
    {
        readonly ILogger<ContinuousJob> logger;
        readonly TelemetryClient telemetryClient;

        public ContinuousJob(ILogger<ContinuousJob> logger, TelemetryClient telemetryClient)
        {
            this.logger = logger;
            this.telemetryClient = telemetryClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var counter = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.Write(".");
                    await Task.Delay(100, stoppingToken);

                    if (counter++ > 20)
                    {
                        logger.LogInformation("!!! About to throw !!!");
                        throw new InvalidOperationException("oy vay!");
                    }
                }
            }
            catch (Exception exception)
            {
                telemetryClient.TrackException(exception);
            }
        }
    }
}