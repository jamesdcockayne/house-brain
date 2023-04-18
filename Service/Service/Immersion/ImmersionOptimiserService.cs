using Microsoft.Extensions.Hosting;

namespace Service.Immersion;

internal class ImmersionOptimiserService : BackgroundService
{
    private readonly ImmersionOptimiser _optimiser;

    public ImmersionOptimiserService(ImmersionOptimiser optimiser)
    {
        _optimiser = optimiser;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _optimiser.OptimiseAsync(stoppingToken);
        }
    }
}