using System.ComponentModel.DataAnnotations;

namespace Service.Gas;

internal class GasHeatingOptions
{
    [Range(10, 100)]
    public int TargetTemperature { get; set; }
}