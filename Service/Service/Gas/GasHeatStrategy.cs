using Microsoft.Extensions.Logging;

namespace Service.Gas;

public class GasHeatStrategy
{
    private readonly IGasCallForHeatRelay _heatCallRelay;
    private readonly ICylinderTemperatureSensor _cylinderSensors;
    private readonly IWait _wait;
    private readonly ILogger<GasHeatStrategy> _logger;
    private readonly IIndirectHeatingIsSaturatedTester _indirectHeatingIsSaturatedTester;

    public GasHeatStrategy(
        IGasCallForHeatRelay heatCallRelay, 
        ICylinderTemperatureSensor cylinderSensors, 
        IWait wait,
        IIndirectHeatingIsSaturatedTester indirectHeatingIsSaturatedTester,
        ILogger<GasHeatStrategy> logger)
    {
        _heatCallRelay = heatCallRelay;
        _cylinderSensors = cylinderSensors;
        _wait = wait;
        _indirectHeatingIsSaturatedTester = indirectHeatingIsSaturatedTester;
        _logger = logger;
    }

    public async Task GasHeatAsync(decimal targetTemp, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting gas heating. Target temp is {}.", targetTemp);

        try
        {
            if (_heatCallRelay.CallForHeat)
            {
                _heatCallRelay.CallForHeat = false;

                throw new InvalidOperationException("This class is not designed to be multi threaded and the relay is already calling for heat. Stopping calling for heat and crashing.");
            }

            if (await TankIsAtTempAsync(targetTemp))
            {
                _logger.LogInformation("Tank is already at temp. No call for heat made.");

                return;
            }

            _heatCallRelay.CallForHeat = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (await TankIsAtTempAsync(targetTemp))
                {
                    _logger.LogInformation("Tank is at temp. Canceling call for heat.");

                    return;
                }

                if (await _indirectHeatingIsSaturatedTester.GasHeatingInletAndOutletTempsAreSimilarAndHotAsync())
                {
                    _logger.LogInformation("Heating flow and returns are too similar. Canceling call for heat.");

                    return;
                }

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
