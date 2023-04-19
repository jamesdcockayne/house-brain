using Microsoft.Extensions.Logging;

namespace Service;

internal class Waiter : IWait
{
    private readonly ILogger<Waiter> _logger;

    public Waiter(ILogger<Waiter> logger)
    {
        _logger = logger;
    }

    public async Task LongWaitAsync(CancellationToken token)
    {
        _logger.LogTrace("Entering long wait.");
        await Task.Delay(5 * 60 * 1000, token);
        _logger.LogTrace("Leaving long wait.");
    }

    public async Task ShortWaitAsync(CancellationToken token)
    {
        _logger.LogTrace("Entering short wait.");
        await Task.Delay(5 * 1000, token);
        _logger.LogTrace("Leaving short wait.");
    }
}