namespace Service;

public class GpioHeaderPinAttribute : System.ComponentModel.DataAnnotations.RangeAttribute
{
    public GpioHeaderPinAttribute()
        : base(1, 40)
    {
    }
}