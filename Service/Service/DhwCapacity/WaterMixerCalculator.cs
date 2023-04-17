namespace Service.DhwCapacity;

public class WaterMixerCalculator
{
    public decimal GetWarmWaterLiters(decimal hotWaterVolume, decimal hotWaterTemp, decimal coldWaterTemp, decimal desiredTemp)
    {
        if (hotWaterTemp < desiredTemp)
            return 0; // If the hot water is colder than the desired temp it will never be warmed by adding cold water.

        // Calculate the amount of heat energy needed to reach the desired temperature
        decimal requiredHeatEnergy = hotWaterVolume * (desiredTemp - hotWaterTemp) * 4.184m;

        // Calculate the mass of cold water needed to supply the required heat energy
        decimal coldWaterMass = requiredHeatEnergy / (4.184m * (desiredTemp - coldWaterTemp));

        return hotWaterVolume + Math.Abs(coldWaterMass);
    }
}