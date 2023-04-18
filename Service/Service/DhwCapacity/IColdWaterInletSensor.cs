namespace Service.DhwCapacity;

public interface IColdWaterInletSensor
{
    Task<decimal> GetColdWaterInletSensorCelsiusAsync();
}