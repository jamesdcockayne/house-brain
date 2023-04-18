namespace Service.Gas;

internal class FakeGasCallForHeatRelay : IGasCallForHeatRelay
{
    public bool CallForHeat { get; set; }
}
