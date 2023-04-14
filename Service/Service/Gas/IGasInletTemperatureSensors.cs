namespace Service.Gas;

public interface IGasInletTemperatureSensors
{
    decimal InletCelsius { get; }
    decimal OutletCelsius { get; }
}
