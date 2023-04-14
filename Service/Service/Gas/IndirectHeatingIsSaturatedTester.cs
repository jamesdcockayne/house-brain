namespace Service.Gas;

public class IndirectHeatingIsSaturatedTester
{
    private readonly IGasInletTemperatureSensors _gasSensor;

    public IndirectHeatingIsSaturatedTester(IGasInletTemperatureSensors gasSensor)
    {
        _gasSensor = gasSensor;
    }

    public bool GasHeatingInletAndOutletTempsAreSimilarAndHot()
    {
        if (IsHot(_gasSensor.InletCelsius) == false)
            return false;

        decimal delta = _gasSensor.InletCelsius - _gasSensor.OutletCelsius;

        return Math.Abs(delta) < 5M;
    }

    private static bool IsHot(decimal temp) => temp > 40;
}