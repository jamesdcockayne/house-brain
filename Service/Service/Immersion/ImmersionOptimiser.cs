using Microsoft.Extensions.Logging;

namespace Service.Immersion;

internal class ImmersionOptimiser
{
    private readonly ICurrentSensor _currentSensor;
    private readonly IImmersionRelay _immersionRelay;
    private readonly IWait _wait;
    private readonly ILogger<ImmersionOptimiser> _logger;

    public ImmersionOptimiser(ICurrentSensor currentSensor, IImmersionRelay immersionRelay, IWait wait, ILogger<ImmersionOptimiser> logger)
    {
        _currentSensor = currentSensor;
        _immersionRelay = immersionRelay;
        _wait = wait;
        _logger = logger;
    }

    public async Task OptimiseAsync(CancellationToken cancellationToken)
    {
        // current flowing top        - leave it, and wait
        // current not flowing top    - try a lower element, if failed move back to upper element and wait
        // current flowing bottom     - try a higher element, if failed move back to lower, and wait
        // current not flowing bottom - leave it, and wait

        bool currentDetected = _currentSensor.CurrentIsDetected;

        _logger.LogTrace("currentDetected {}", currentDetected);

        if (_immersionRelay.TopImmersionEnabled)
        {
            if (currentDetected)
            {
                _logger.LogInformation("Top element enabled, current detected.");

                await _wait.LongWaitAsync(cancellationToken);
            }
            else
            {
                _logger.LogInformation("Top element enabled, no current detected. Try lower element.");

                await TryImmersionAndWait(isTopElement: false, cancellationToken);
            }
        }
        else
        {
            if (currentDetected)
            {
                _logger.LogInformation("Top element disabled, current detected. Trying top element.");

                await TryImmersionAndWait(isTopElement: true, cancellationToken);
            }
            else
            {
                _logger.LogInformation("Top element disabled, no current detected. Either no power or water is at temp.");

                await _wait.LongWaitAsync(cancellationToken);
            }
        } 
    }

    private async Task TryImmersionAndWait(bool isTopElement, CancellationToken cancellationToken)
    {
        _immersionRelay.TopImmersionEnabled = isTopElement;

        if (_currentSensor.CurrentIsDetected)
        {
            _logger.LogInformation("Swapped to other element.");
            await _wait.LongWaitAsync(cancellationToken);
            return;
        }

        _logger.LogInformation("Returning to original element.");

        _immersionRelay.TopImmersionEnabled = !isTopElement;

        await _wait.LongWaitAsync(cancellationToken);
    }
}