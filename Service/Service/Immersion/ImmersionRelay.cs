using System.Device.Gpio;
using Microsoft.Extensions.Options;

namespace Service.Immersion;

public class ImmersionRelay : IImmersionRelay, IDisposable
{
    private bool disposedValue;
    private readonly GpioController _gpioController;
    private readonly WiringPinOptions _wiringPinOptions;

    public ImmersionRelay(IOptions<WiringPinOptions> options)
    {
        _gpioController = new GpioController(PinNumberingScheme.Logical);
        _wiringPinOptions = options.Value;

        _gpioController.OpenPin(options.Value.ImmersionRelayPinNumber, PinMode.Output);
    }


    public bool TopImmersionEnabled
    {
        get => _gpioController.Read(_wiringPinOptions.ImmersionRelayPinNumber) == PinValue.High;
        set
        {
            PinValue pinValue = value ? PinValue.High : PinValue.Low;

            _gpioController.Write(_wiringPinOptions.ImmersionRelayPinNumber, pinValue);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _gpioController.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}