using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Service.Immersion;

internal class ImmersionOptimiserService : BackgroundService
{
    private readonly ImmersionOptimiser _optimiser;
    private readonly ILogger<ImmersionOptimiserService> _logger;

    public ImmersionOptimiserService(ImmersionOptimiser optimiser, ILogger<ImmersionOptimiserService> logger)
    {
        _optimiser = optimiser;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting ImmersionOptimiser");

        while (!stoppingToken.IsCancellationRequested)
        {
            await _optimiser.OptimiseAsync(stoppingToken);
        }
    }
}