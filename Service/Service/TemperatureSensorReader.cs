using Microsoft.Extensions.Options;
using Iot.Device.OneWire;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Service;

public class TemperatureSensorReader : ICylinderTemperatureSensor, Gas.IGasInletTemperatureSensors, DhwCapacity.IColdWaterInletSensor
{
    private readonly object _lock = new();
    private readonly Dictionary<string, decimal> TemperatureBySensor = new();
    private readonly TemperatureSensorOptions _options;
    private readonly ILogger<TemperatureSensorReader> _logger;

    public TemperatureSensorReader(IOptions<TemperatureSensorOptions> options, ILogger<TemperatureSensorReader> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<decimal> GetColdWaterInletSensorCelsiusAsync()
    {
        var temp = await GetLastReadingAsync(_options.ColdWaterSensorId!);

        _logger.LogDebug("Cold water temp {}", temp);

        return temp;
    }

    public async Task<decimal> GetFlowCelsiusAsync()
    {
        var temp = await GetLastReadingAsync(_options.FlowSensorId!);

        _logger.LogDebug("Flow temp {}", temp);

        return temp;
    }

    public async Task<decimal> GetReturnCelsiusAsync()
    {
        var temp = await GetLastReadingAsync(_options.ReturnSensorId!);

        _logger.LogDebug("Return temp {}", temp);

        return temp;
    }

    public async Task<decimal[]> GetSensorsAsync()
    {
        var readings = new List<decimal>();

        foreach (var sensorId in _options.TankSensorIds!)
            readings.Add(await GetLastReadingAsync(sensorId));

        _logger.LogDebug("Tank temp (top to bottom) {}", string.Join(", ", readings.Cast<int>()));

        return readings.ToArray();
    }

    private async Task<decimal> GetLastReadingAsync(string sensorId)
    {
        decimal temp;
        int tryCount = 0;
        const int ThirtySeconds = 30;

        while (!TryGetLastReading(sensorId, out temp))
        {
            await Task.Delay(1000);
            tryCount++;

            _logger.LogTrace("Waiting for reading. Sensor id {}, try number {}", sensorId, tryCount);

            if (tryCount >= ThirtySeconds)
                throw new InvalidOperationException($"Could not get sensor reading for {sensorId}.");
        }

        return temp;
    }

    private bool TryGetLastReading(string sensorId, out decimal temp)
    {
        lock (_lock)
        {
            return TemperatureBySensor.TryGetValue(sensorId, out temp);
        }
    }

    public async Task MonitorSensorsAsync(CancellationToken token)
    {
        _logger.LogInformation("Monitoring beginning.");

        while (!token.IsCancellationRequested)
        {
            var stopWatch = Stopwatch.StartNew();

            await TakeReadingsAsync();

            _logger.LogTrace("Scan took {} milliseconds.", stopWatch.ElapsedMilliseconds);
        }
    }

    private async Task TakeReadingsAsync()
    {
        foreach (string busId in OneWireBus.EnumerateBusIds())
        {
            _logger.LogTrace("Scanning bus {}", busId);

            OneWireBus bus = new(busId);

            await bus.ScanForDeviceChangesAsync();

            foreach (string devId in bus.EnumerateDeviceIds())
            {
                OneWireDevice dev = new(busId, devId);

                if (OneWireThermometerDevice.IsCompatible(busId, devId))
                {
                    OneWireThermometerDevice devTemp = new(busId, devId);

                    var temperature = await devTemp.ReadTemperatureAsync();

                    _logger.LogTrace("Sensor {} on bus {} is {}c", devId, busId, (decimal)temperature.DegreesCelsius);

                    lock (_lock)
                    {
                        TemperatureBySensor[devId] = (decimal)temperature.DegreesCelsius;
                    }   
                }
            }
        }
    }
}