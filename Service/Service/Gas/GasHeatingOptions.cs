using System.ComponentModel.DataAnnotations;

namespace Service.Gas;

public class GasHeatingOptions
{
    [Range(10, 100)]
    public int TargetTemperature { get; set; }
}