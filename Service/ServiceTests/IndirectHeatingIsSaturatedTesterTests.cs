using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Gas;

namespace ServiceTests;

[TestClass]
public class IndirectHeatingIsSaturatedTesterTests
{

    [TestMethod]
    public void TestSimilarTempsAtLowTempIsFalse()
    {
        Mock<IGasInletTemperatureSensors> sensorMock = new Mock<IGasInletTemperatureSensors>();

        sensorMock.SetupGet(mock => mock.InletCelsius).Returns(10);
        sensorMock.SetupGet(mock => mock.InletCelsius).Returns(11);

        var tester = new IndirectHeatingIsSaturatedTester(sensorMock.Object);

        Assert.IsFalse(tester.GasHeatingInletAndOutletTempsAreSimilarAndHot());
    }

    [TestMethod]
    public void TestSimilarTempsAtHighTempIsTrue()
    {
        Mock<IGasInletTemperatureSensors> sensorMock = new Mock<IGasInletTemperatureSensors>();

        sensorMock.SetupGet(mock => mock.InletCelsius).Returns(82);
        sensorMock.SetupGet(mock => mock.InletCelsius).Returns(84);

        var tester = new IndirectHeatingIsSaturatedTester(sensorMock.Object);

        Assert.IsFalse(tester.GasHeatingInletAndOutletTempsAreSimilarAndHot());
    }

    [TestMethod]
    public void TestDisimilarTempsAtHighTempIsFalse()
    {
        Mock<IGasInletTemperatureSensors> sensorMock = new Mock<IGasInletTemperatureSensors>();

        sensorMock.SetupGet(mock => mock.InletCelsius).Returns(84);
        sensorMock.SetupGet(mock => mock.InletCelsius).Returns(70);

        var tester = new IndirectHeatingIsSaturatedTester(sensorMock.Object);

        Assert.IsFalse(tester.GasHeatingInletAndOutletTempsAreSimilarAndHot());
    }

    [TestMethod]
    public void TestDisimilarTempsAtHighTempIsFalseWithBackwardsSensors()
    {
        Mock<IGasInletTemperatureSensors> sensorMock = new Mock<IGasInletTemperatureSensors>();

        sensorMock.SetupGet(mock => mock.InletCelsius).Returns(70);
        sensorMock.SetupGet(mock => mock.InletCelsius).Returns(84);

        var tester = new IndirectHeatingIsSaturatedTester(sensorMock.Object);

        Assert.IsFalse(tester.GasHeatingInletAndOutletTempsAreSimilarAndHot());
    }
}