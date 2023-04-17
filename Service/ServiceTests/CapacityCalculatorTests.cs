using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using Moq;
using Service.DhwCapacity;
using Microsoft.Extensions.Options;

namespace ServiceTests;

[TestClass]
public class CapacityCalculatorTests
{
    [TestMethod]
    public void TestOnePartitionOfTank()
    {
        var cylinderTempSensorMock = new Mock<ICylinderTemperatureSensor>();

        cylinderTempSensorMock.Setup(mock => mock.Sensors).Returns(new decimal[] { 0, 0, 0, 0, 100 });

        var inletSensorMock = new Mock<IColdWaterInletSensor>();

        inletSensorMock.Setup(mock => mock.ColdWaterInletSensorCelsius).Returns(0);

        var optionsMock = new Mock<IOptions<CapacityCalculatorOptions>>();

        optionsMock.Setup(mock => mock.Value).Returns(new CapacityCalculatorOptions { TargetOutletTemperature = 50, TankCapacityLiters = 100 });

        var calculator =
            new CapacityCalculator(
                cylinderTempSensorMock.Object,
                inletSensorMock.Object,
                optionsMock.Object,
                new WaterMixerCalculator());

        Assert
            .AreEqual(
                expected: 40, // 1/5th of the tank is 20L. Inlet is 0c and tank is 100c
                actual: calculator.Capacity);
    }

    [TestMethod]
    public void TestMultiPartitionOfTank()
    {
        var cylinderTempSensorMock = new Mock<ICylinderTemperatureSensor>();

        cylinderTempSensorMock.Setup(mock => mock.Sensors).Returns(new decimal[] { 0, 0, 0, 100, 100 });

        var inletSensorMock = new Mock<IColdWaterInletSensor>();

        inletSensorMock.Setup(mock => mock.ColdWaterInletSensorCelsius).Returns(0);

        var optionsMock = new Mock<IOptions<CapacityCalculatorOptions>>();

        optionsMock.Setup(mock => mock.Value).Returns(new CapacityCalculatorOptions { TargetOutletTemperature = 50, TankCapacityLiters = 100 });

        var calculator =
            new CapacityCalculator(
                cylinderTempSensorMock.Object,
                inletSensorMock.Object,
                optionsMock.Object,
                new WaterMixerCalculator());

        Assert
            .AreEqual(
                expected: 80, // 2/5th of the tank is 40L. Inlet is 0c and tank is 100c
                actual: calculator.Capacity);
    }
}