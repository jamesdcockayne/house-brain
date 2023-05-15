using Microsoft.Extensions.Options;

namespace Service.DhwCapacity;

public class CapacityCalculator
{
    private readonly ICylinderTemperatureSensor _cylinderTemperatureSensors;
    private readonly IColdWaterInletSensor _coldWaterInletSensor;
    private readonly CapacityCalculatorOptions _calculatorOptions;
    private readonly WaterMixerCalculator _waterMixerCalculator;
    private readonly decimal _hotWaterPartitionVolumeLiters;

    public CapacityCalculator(
        ICylinderTemperatureSensor cylinderTemperatureSensors, 
        IColdWaterInletSensor coldWaterInletSensor, 
        IOptions<CapacityCalculatorOptions> options,
        WaterMixerCalculator waterMixerCalculator)
    {
        _cylinderTemperatureSensors = cylinderTemperatureSensors;
        _coldWaterInletSensor = coldWaterInletSensor;
        _calculatorOptions = options.Value;
        _waterMixerCalculator = waterMixerCalculator;

        _hotWaterPartitionVolumeLiters = options.Value.TankCapacityLiters / options.Value.TankSensorCount;
    }

    public async Task<decimal> GetCapacityAsync()
    {
        var sensors = await _cylinderTemperatureSensors.GetSensorsAsync();

        var tasks = sensors.Select(GetPartitionWarmWaterLitersAsync).ToList();

        await Task.WhenAll(tasks);

        return tasks.Sum(task => task.Result);
    }

    private async Task<decimal> GetPartitionWarmWaterLitersAsync(decimal hotWaterTemp)
    {
        var coldWaterTemp = await _coldWaterInletSensor.GetColdWaterInletSensorCelsiusAsync();
            
        return 
            _waterMixerCalculator
            .GetWarmWaterLiters(
                hotWaterTemp: hotWaterTemp,
                hotWaterVolume: _hotWaterPartitionVolumeLiters,
                coldWaterTemp: coldWaterTemp,
                desiredTemp: _calculatorOptions.TargetOutletTemperature);
    }            
}