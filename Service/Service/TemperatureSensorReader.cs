using Microsoft.Extensions.Options;
using Iot.Device.OneWire;

namespace Service;

public class TemperatureSensorReader : ICylinderTemperatureSensor, Gas.IGasInletTemperatureSensors, DhwCapacity.IColdWaterInletSensor
{
    private readonly object _lock = new();
    private readonly Dictionary<string, decimal> TemperatureBySensor = new();
    private readonly TemperatureSensorOptions _options;

    public TemperatureSensorReader(IOptions<TemperatureSensorOptions> options)
    {
        _options = options.Value;
    }

    public async Task<decimal> GetColdWaterInletSensorCelsiusAsync()
    {
        return await GetLastReadingAsync(_options.ColdWaterSensorId!);
    }

    public async Task<decimal> GetFlowCelsiusAsync()
    {
        return await GetLastReadingAsync(_options.FlowSensorId!);
    }

    public async Task<decimal> GetReturnCelsiusAsync()
    {
        return await GetLastReadingAsync(_options.ReturnSensorId!);
    }

    public async Task<decimal[]> GetSensorsAsync()
    {
        var readings = new List<decimal>();

        foreach (var sensorId in _options.TankSensorIds!)
            readings.Add(await GetLastReadingAsync(sensorId));

        return readings.ToArray();
    }

    private async Task<decimal> GetLastReadingAsync(string sensorId)
    {
        decimal temp;
        int tryCount = 0;
        const int ThirtySeconds = 30 * 1000;

        while (!TryGetLastReading(sensorId, out temp))
        {
            await Task.Delay(1);
            tryCount++;

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
        while (!token.IsCancellationRequested)
        {
            await TakeReadingsAsync();
        }
    }

    private async Task TakeReadingsAsync()
    {
        foreach (string busId in OneWireBus.EnumerateBusIds())
        {
            OneWireBus bus = new(busId);

            await bus.ScanForDeviceChangesAsync();

            foreach (string devId in bus.EnumerateDeviceIds())
            {
                OneWireDevice dev = new(busId, devId);

                if (OneWireThermometerDevice.IsCompatible(busId, devId))
                {
                    OneWireThermometerDevice devTemp = new(busId, devId);

                    var temperature = await devTemp.ReadTemperatureAsync();

                    lock (_lock)
                    {
                        TemperatureBySensor[devId] = (decimal)temperature.DegreesCelsius;
                    }   
                }
            }
        }
    }
}
