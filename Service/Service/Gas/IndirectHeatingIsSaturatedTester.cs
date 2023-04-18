namespace Service.Gas;

public class IndirectHeatingIsSaturatedTester
{
    private readonly IGasInletTemperatureSensors _gasSensor;

    public IndirectHeatingIsSaturatedTester(IGasInletTemperatureSensors gasSensor)
    {
        _gasSensor = gasSensor;
    }

    public async Task<bool> GasHeatingInletAndOutletTempsAreSimilarAndHotAsync()
    {
        if (IsHot(await _gasSensor.GetFlowCelsiusAsync()) == false)
            return false;

        decimal delta = await _gasSensor.GetFlowCelsiusAsync() - await _gasSensor.GetReturnCelsiusAsync();

        return Math.Abs(delta) < 5M;
    }

    private static bool IsHot(decimal temp) => temp > 40;
}