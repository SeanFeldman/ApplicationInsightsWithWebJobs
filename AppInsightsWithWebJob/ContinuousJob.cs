using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppInsightsWithWebJob
{
    public class ContinuousJob : BackgroundService
    {
        private readonly ILogger<ContinuousJob> logger;

        public ContinuousJob(ILogger<ContinuousJob> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
    }
}