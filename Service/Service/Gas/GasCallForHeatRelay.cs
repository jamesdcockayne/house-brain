using System.Device.Gpio;
using Microsoft.Extensions.Options;

namespace Service.Gas;

public class GasCallForHeatRelay : IGasCallForHeatRelay, IDisposable
{
    private bool disposedValue;
    private readonly GpioController _gpioController;
    private readonly WiringPinOptions _wiringPinOptions;

    public GasCallForHeatRelay(IOptions<WiringPinOptions> options)
    {
        _gpioController = new GpioController(PinNumberingScheme.Logical);
        _wiringPinOptions = options.Value;

        _gpioController.OpenPin(options.Value.GasCallForHeatRelayPinNumber, PinMode.Output);
    }

    public bool CallForHeat
    {
        get => _gpioController.Read(_wiringPinOptions.GasCallForHeatRelayPinNumber) == PinValue.High;
        set
        {
            PinValue pinValue = value ? PinValue.High : PinValue.Low;

            _gpioController.Write(_wiringPinOptions.GasCallForHeatRelayPinNumber, pinValue);
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