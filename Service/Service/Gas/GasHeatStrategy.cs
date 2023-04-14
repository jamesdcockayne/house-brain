namespace Service.Gas;

public class GasHeatStrategy
{
    private readonly IGasCallForHeatRelay _heatCallRelay;
    private readonly ICylinderTemperatureSensor _cylinderSensors;
    private readonly IWait _wait;

    public GasHeatStrategy(IGasCallForHeatRelay heatCallRelay, ICylinderTemperatureSensor cylinderSensors, IWait wait)
    {
        _heatCallRelay = heatCallRelay;
        _cylinderSensors = cylinderSensors;
        _wait = wait;
    }

    public async Task GasHeatAsync(decimal targetTemp, CancellationToken cancellationToken)
    {
        try
        {
            if (_heatCallRelay.CallForHeat)
            {
                _heatCallRelay.CallForHeat = false;

                throw new InvalidOperationException("This class is not designed to be multi threaded and the relay is already calling for heat. Stopping calling for heat and crashing.");
            }

            if (TankIsAtTemp(targetTemp))
                return;

            _heatCallRelay.CallForHeat = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (TankIsAtTemp(targetTemp))
                    return;

                await _wait.ShortWait();
            }
        }
        finally
        {
            _heatCallRelay.CallForHeat = false;
        }
    }

    private bool TankIsAtTemp(decimal targetTemp) => _cylinderSensors.SensorCelsius0 >= targetTemp;
}
