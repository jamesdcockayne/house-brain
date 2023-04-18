using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using Moq;
using Service.DhwCapacity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ServiceTests;

[TestClass]
public class CapacityCalculatorTests
{
    [TestMethod]
    public async Task TestOnePartitionOfTank()
    {
        var cylinderTempSensorMock = new Mock<ICylinderTemperatureSensor>();

        cylinderTempSensorMock.Setup(mock => mock.GetSensorsAsync()).ReturnsAsync(new decimal[] { 0, 0, 0, 0, 100 });

        var inletSensorMock = new Mock<IColdWaterInletSensor>();

        inletSensorMock.Setup(mock => mock.GetColdWaterInletSensorCelsiusAsync()).ReturnsAsync(0);

        var optionsMock = new Mock<IOptions<CapacityCalculatorOptions>>();

        optionsMock.Setup(mock => mock.Value).Returns(new CapacityCalculatorOptions { TargetOutletTemperature = 50, TankCapacityLiters = 100, TankSensorCount = 5 });

        var calculator =
            new CapacityCalculator(
                cylinderTempSensorMock.Object,
                inletSensorMock.Object,
                optionsMock.Object,
                new WaterMixerCalculator());

        Assert
            .AreEqual(
                expected: 40, // 1/5th of the tank is 20L. Inlet is 0c and tank is 100c
                actual: await calculator.GetCapacityAsync());
    }

    [TestMethod]
    public async Task TestMultiPartitionOfTank()
    {
        var cylinderTempSensorMock = new Mock<ICylinderTemperatureSensor>();

        cylinderTempSensorMock.Setup(mock => mock.GetSensorsAsync()).ReturnsAsync(new decimal[] { 0, 0, 0, 100, 100 });

        var inletSensorMock = new Mock<IColdWaterInletSensor>();

        inletSensorMock.Setup(mock => mock.GetColdWaterInletSensorCelsiusAsync()).ReturnsAsync(0);

        var optionsMock = new Mock<IOptions<CapacityCalculatorOptions>>();

        optionsMock.Setup(mock => mock.Value).Returns(new CapacityCalculatorOptions { TargetOutletTemperature = 50, TankCapacityLiters = 100, TankSensorCount = 5 });

        var calculator =
            new CapacityCalculator(
                cylinderTempSensorMock.Object,
                inletSensorMock.Object,
                optionsMock.Object,
                new WaterMixerCalculator());

        Assert
            .AreEqual(
                expected: 80, // 2/5th of the tank is 40L. Inlet is 0c and tank is 100c
                actual: await calculator.GetCapacityAsync());
    }
}