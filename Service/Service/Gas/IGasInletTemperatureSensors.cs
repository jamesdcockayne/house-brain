namespace Service.Gas;

public interface IGasInletTemperatureSensors
{
    Task<decimal> GetFlowCelsiusAsync();
    Task<decimal> GetReturnCelsiusAsync();
}
