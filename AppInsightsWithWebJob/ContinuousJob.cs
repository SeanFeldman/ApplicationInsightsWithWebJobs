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
            logger.LogInformation("executing WebJob");
            try
            {
                var counter = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.Write(".");
                    await Task.Delay(150, stoppingToken);
                    logger.LogDebug($"Counter is at {counter}", counter);

                    if (counter++ > 10)
                    {
                        logger.LogWarning(">> About to throw <<");
                        throw new InvalidOperationException("oy vay!");
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogCritical("[JOB] Continuous job threw an exceptions. {0}", exception);
                telemetryClient.TrackException(exception);
                await Task.Delay(2_000); // allow AppInsights to send the data???
            }
        }
    }
}