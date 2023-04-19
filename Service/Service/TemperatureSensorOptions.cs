using System.ComponentModel.DataAnnotations;

namespace Service;

public class TemperatureSensorOptions
{
    [Required]
    public string? FlowSensorId { get; set; }

    [Required]
    public string? ReturnSensorId { get; set; }

    [Required]
    public string? ColdWaterSensorId { get; set; }

    [Required]
    public List<string>? TankSensorIds { get; set; }
}