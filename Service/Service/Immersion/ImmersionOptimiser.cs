namespace Service.Immersion;

internal class ImmersionOptimiser
{
    private readonly ICurrentSensor _currentSensor;
    private readonly IImmersionRelay _immersionRelay;
    private readonly IWait _wait;

    public ImmersionOptimiser(ICurrentSensor currentSensor, IImmersionRelay immersionRelay, IWait wait)
    {
        _currentSensor = currentSensor;
        _immersionRelay = immersionRelay;
        _wait = wait;
    }

    public async Task OptimiseAsync()
    {
        // current flowing top        - leave it, and wait
        // current not flowing top    - try a lower element, if failed move back to upper element and wait
        // current flowing bottom     - try a higher element, if failed move back to lower, and wait
        // current not flowing bottom - leave it, and wait

        bool currentDetected = _currentSensor.CurrentIsDetected;

        if (_immersionRelay.TopImmersionEnabled)
        {
            if (currentDetected)
            {
                await _wait.LongWait();
            }
            else
            {
                await TryImmersionAndWait(isTopElement: false);
            }
        }
        else
        {
            if (currentDetected)
            {
                await TryImmersionAndWait(isTopElement: true);
            }
            else
            {
                await _wait.LongWait();
            }
        } 
    }

    private async Task TryImmersionAndWait(bool isTopElement)
    {
        _immersionRelay.TopImmersionEnabled = isTopElement;

        if (_currentSensor.CurrentIsDetected)
        {
            await _wait.LongWait();
            return;
        }

        _immersionRelay.TopImmersionEnabled = !isTopElement;

        await _wait.LongWait();
    }
}