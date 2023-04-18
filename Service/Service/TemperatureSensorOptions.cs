namespace Service;

public class TemperatureSensorOptions
{
    public string? FlowSensorId { get; set; }
    public string? ReturnSensorId { get; set; }
    public string? ColdWaterSensorId { get; set; }
    public List<string>? TankSensorIds { get; set; }
}