namespace Service;

public interface ICylinderTemperatureSensor
{
    decimal SensorCelsius0 { get; } // lowest sensor on the tank
    decimal SensorCelsius1 { get; }
    decimal SensorCelsius2 { get; }
    decimal SensorCelsius3 { get; }
    decimal SensorCelsius4 { get; }
}
