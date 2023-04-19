using System.ComponentModel.DataAnnotations;

namespace Service.DhwCapacity;

public class CapacityCalculatorOptions
{
    [Range(10, 100)]
    public decimal TargetOutletTemperature { get; set; }

    [Range(1, 1000)]
    public decimal TankCapacityLiters { get; set; }

    [Range(1, 100)]
    public int TankSensorCount { get; set; }
}
