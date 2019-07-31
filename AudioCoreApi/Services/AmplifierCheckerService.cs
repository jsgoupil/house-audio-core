using AudioCoreSerial.I;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AudioCoreApi.Services
{
    public class AmplifierCheckerService : IHostedService, IDisposable
    {
        private readonly ILogger logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IAmplifier amplifier;
        private readonly ICommunication communication;
        private Timer timer;

        private bool isAlive = true; // We say we start as alive.

        public AmplifierCheckerService(
            ILogger<AmplifierCheckerService> logger,
            IServiceScopeFactory scopeFactory,
            IAmplifier amplifier,
            ICommunication communication
        )
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
            this.amplifier = amplifier;
            this.communication = communication;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Background Service is starting");

            timer = new Timer(Execute, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Background Service is stopping");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void Execute(object state)
        {
            logger.LogInformation("Timed Background Service is working");
            Task.Run(async () =>
            {
                var isAmplifierResponding = await IsAmplifierRespondingAsync();
                logger.LogInformation("The amplifier is {RESPONDING}", isAmplifierResponding);
                if (!isAlive && isAmplifierResponding)
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var resetService = scope.ServiceProvider.GetRequiredService<ResetService>();

                        // We send a reset because we just came back.
                        try
                        {
                            logger.LogInformation("Sending RESET to the amplifier");
                            await resetService.ResetAsync();
                            isAlive = true;
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning(ex, "Sending reset did not work");
                        }
                    }
                }
                else if (isAlive && !isAmplifierResponding)
                {
                    logger.LogInformation("The amplifier just stopped responding");
                    isAlive = false;
                }
            }).GetAwaiter().GetResult();
        }

        private async Task<bool> IsAmplifierRespondingAsync()
        {
            var result = string.Empty;
            if (isAlive)
            {
                // We are alive, so we make a simple request to see if it responds
                try
                {
                    logger.LogTrace("Getting the version");
                    result = await amplifier.GetVersionAsync();
                }
                catch (Exception)
                {
                    logger.LogTrace("Version did not work");
                }
            }
            else
            {
                // If we are not alive, we try to get a string, when the amplifier will come back it will talk to us.
                logger.LogTrace("Getting simple read");
                result = await communication.ReadUntilTimeoutAsync();
            }

            logger.LogTrace("We got: {RESULT}", result);
            return result?.Length > 0;
        }
    }
}