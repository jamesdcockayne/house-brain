using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Service.Gas;

internal class GasHeatSchedulerService : BackgroundService
{
    private readonly GasHeatStrategy _gasHeatStrategy;
    private readonly GasHeatingOptions _gasHeatingOptions;
    private readonly ILogger<GasHeatSchedulerService> _logger;

    public GasHeatSchedulerService(GasHeatStrategy gasHeatStrategy, IOptions<GasHeatingOptions> options, ILogger<GasHeatSchedulerService> logger)
    {
        _gasHeatStrategy = gasHeatStrategy;
        _gasHeatingOptions = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting");

        await WaitUntilTommrowMorningAsync();

        using PeriodicTimer timer = new(TimeSpan.FromDays(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                CancellationTokenSource timeoutTokenSource = new CancellationTokenSource(new TimeSpan(hours: 1, minutes: 0, seconds: 0));

                var timeoutToken = timeoutTokenSource.Token;

                var source = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, timeoutToken);

                try
                {
                    await _gasHeatStrategy.GasHeatAsync((decimal)_gasHeatingOptions.TargetTemperature, source.Token);
                }
                catch (OperationCanceledException)
                {
                    // In this case the timeout token was cancelled because the gas heating ran for too long, rather than the overall worker being cancelled (eg in the case where the app is shutting down).
                    if (timeoutToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Gas heating timed out.");
                    }

                    throw;
                }
                _logger.LogInformation("Waiting for 24 hours.");
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task WaitUntilTommrowMorningAsync()
    {
        _logger.LogInformation("Waiting until early morning.");

        var tommrow = DateTime.UtcNow.AddDays(1);

        var tomorrowMorning = new DateTime(year: tommrow.Year, tommrow.Month, tommrow.Hour, hour: 5, minute: 0, second: 0);

        var interval = DateTime.UtcNow - tomorrowMorning;

        await Task.Delay(interval);
    }
}