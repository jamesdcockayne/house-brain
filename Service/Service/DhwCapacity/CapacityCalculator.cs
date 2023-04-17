using Microsoft.Extensions.Options;

namespace Service.DhwCapacity
{
    public class CapacityCalculator
    {
        private readonly ICylinderTemperatureSensor _cylinderTemperatureSensors;
        private readonly IColdWaterInletSensor _coldWaterInletSensor;
        private readonly CapacityCalculatorOptions _calculatorOptions;
        private readonly WaterMixerCalculator _waterMixerCalculator;

        private decimal HotWaterPartitionVolumeLiters { get; init; }

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

            HotWaterPartitionVolumeLiters = options.Value.TankCapacityLiters / cylinderTemperatureSensors.Sensors.Length;
        }

        public decimal Capacity =>
            _cylinderTemperatureSensors
            .Sensors
            .Sum(
                sensor => 
                    _waterMixerCalculator
                    .GetWarmWaterLiters(
                        hotWaterTemp: sensor,
                        hotWaterVolume: HotWaterPartitionVolumeLiters,
                        coldWaterTemp: _coldWaterInletSensor.ColdWaterInletSensorCelsius,
                        desiredTemp: _calculatorOptions.TargetOutletTemperature));
    }
}