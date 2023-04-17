using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.DhwCapacity;

namespace ServiceTests;

[TestClass]
public class WaterMixerCalculatorTests
{
    [TestMethod]
    public void TestMixFreezingAndBoilingWater()
    {
        var calculator = new WaterMixerCalculator();

        var totalVolume = 
            calculator
            .GetWarmWaterLiters(
                hotWaterTemp: 100,
                hotWaterVolume: 10,
                coldWaterTemp: 0,
                desiredTemp: 50);

        Assert.AreEqual(20, totalVolume);
    }

    [TestMethod]
    public void TestHotWaterColderThanDesiredIsZero()
    {
        var calculator = new WaterMixerCalculator();

        var totalVolume =
            calculator
            .GetWarmWaterLiters(
                hotWaterTemp: 100,
                hotWaterVolume: 10,
                coldWaterTemp: 14,
                desiredTemp: 150);

        Assert.AreEqual(0, totalVolume);
    }
}