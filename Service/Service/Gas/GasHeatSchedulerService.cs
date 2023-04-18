using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Service.Gas;

internal class GasHeatSchedulerService : BackgroundService
{
    private readonly GasHeatStrategy _gasHeatStrategy;
    private readonly GasHeatingOptions _gasHeatingOptions;

    public GasHeatSchedulerService(GasHeatStrategy gasHeatStrategy, IOptions<GasHeatingOptions> options)
    {
        _gasHeatStrategy = gasHeatStrategy;
        _gasHeatingOptions = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitUntilTommrowMorningAsync();

        using PeriodicTimer timer = new(TimeSpan.FromDays(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await _gasHeatStrategy.GasHeatAsync(_gasHeatingOptions.TargetTemperature, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task WaitUntilTommrowMorningAsync()
    {
        var tommrow = DateTime.UtcNow.AddDays(1);

        var tomorrowMorning = new DateTime(year: tommrow.Year, tommrow.Month, tommrow.Hour, hour: 5, minute: 0, second: 0);

        var interval = DateTime.UtcNow - tomorrowMorning;

        await Task.Delay(interval);
    }
}