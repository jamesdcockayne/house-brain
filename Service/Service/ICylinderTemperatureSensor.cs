namespace Service;

public interface ICylinderTemperatureSensor
{
    Task<decimal[]> GetSensorsAsync(); // top to bottom
}
