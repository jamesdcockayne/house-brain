﻿namespace Service.Gas;

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

            if (await TankIsAtTempAsync(targetTemp))
                return;

            _heatCallRelay.CallForHeat = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (await TankIsAtTempAsync(targetTemp))
                    return;

                await _wait.ShortWaitAsync(cancellationToken);
            }
        }
        finally
        {
            _heatCallRelay.CallForHeat = false;
        }
    }

    private async Task<bool> TankIsAtTempAsync(decimal targetTemp) => (await _cylinderSensors.GetSensorsAsync()).Last() >= targetTemp;
}
