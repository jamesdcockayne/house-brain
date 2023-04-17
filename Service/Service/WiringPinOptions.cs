namespace Service;

public class WiringPinOptions
{
    public int ImmersionRelayPinNumber { get; set; }
    public int GasCallForHeatRelayPinNumber { get; set; }
}

// epaper hat
//VCC    ->    3.3
//GND    ->    GND
//DIN    ->    10(SPI0_MOSI)
//CLK    ->    11(SPI0_SCK)
//CS     ->    8(SPI0_CS0)
//DC     ->    25
//RST    ->    17
//BUSY   ->    24
// https://github.com/waveshare/e-Paper/blob/master/RaspberryPi_JetsonNano/python/readme_rpi_EN.txt