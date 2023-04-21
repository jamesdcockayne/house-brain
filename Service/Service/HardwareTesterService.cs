using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Gas;
using Service.Immersion;

namespace Service;

internal class HardwareTesterService : BackgroundService
{
    private readonly TemperatureSensorReader _temperatureSensorReader;
    private readonly Service.Immersion.IImmersionRelay _immersionRelay;
    private readonly Service.Gas.IGasCallForHeatRelay _gasCallForHeatRelay;
    private readonly Service.Immersion.ICurrentSensor _currentSensor;
    private readonly ILogger<HardwareTesterService> _logger;

    public HardwareTesterService(TemperatureSensorReader temperatureSensorReader, IImmersionRelay immersionRelay, IGasCallForHeatRelay gasCallForHeatRelay, ICurrentSensor currentSensor, ILogger<HardwareTesterService> logger)
    {
        _temperatureSensorReader = temperatureSensorReader;
        _immersionRelay = immersionRelay;
        _gasCallForHeatRelay = gasCallForHeatRelay;
        _currentSensor = currentSensor;
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting hardware test mode.");

        // Start the temp sensor loop.
        _ = Task.Run(() => _temperatureSensorReader.MonitorSensorsAsync(stoppingToken), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await _temperatureSensorReader.GetColdWaterInletSensorCelsiusAsync();
            await _temperatureSensorReader.GetFlowCelsiusAsync();
            await _temperatureSensorReader.GetReturnCelsiusAsync();
            await _temperatureSensorReader.GetSensorsAsync();

            _immersionRelay.TopImmersionEnabled = true;

            await Task.Delay(2000, stoppingToken);

            _immersionRelay.TopImmersionEnabled = false;

            await Task.Delay(2000, stoppingToken);

            _gasCallForHeatRelay.CallForHeat = true;

            await Task.Delay(2000, stoppingToken);

            _gasCallForHeatRelay.CallForHeat = false;

            await Task.Delay(2000, stoppingToken);

            // todo _currentSensor.CurrentIsDetected;

            await Task.Delay(2000, stoppingToken);
        }
    }
}
