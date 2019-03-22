﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppInsightsWithWebJob
{
    public class ContinuousJob : IHostedService
    {
        private readonly ILogger<ContinuousJob> logger;

        public ContinuousJob(ILogger<ContinuousJob> logger)
        {
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                var counter = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.Write(".");
                    await Task.Delay(100, cancellationToken);

                    if (counter++ > 20)
                    {
                        logger.LogInformation("<<< About to throw >>>");
                        throw new InvalidOperationException("oy vay!");
                    }
                }
            },TaskCreationOptions.LongRunning);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}