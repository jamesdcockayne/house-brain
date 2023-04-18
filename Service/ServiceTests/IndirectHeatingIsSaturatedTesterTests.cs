using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Gas;
using System.Threading.Tasks;

namespace ServiceTests;

[TestClass]
public class IndirectHeatingIsSaturatedTesterTests
{

    [TestMethod]
    public async Task TestSimilarTempsAtLowTempIsFalse()
    {
        Mock<IGasInletTemperatureSensors> sensorMock = new Mock<IGasInletTemperatureSensors>();

        sensorMock.Setup(mock => mock.GetFlowCelsiusAsync()).ReturnsAsync(10);
        sensorMock.Setup(mock => mock.GetReturnCelsiusAsync()).ReturnsAsync(11);

        var tester = new IndirectHeatingIsSaturatedTester(sensorMock.Object);

        Assert.IsFalse(await tester.GasHeatingInletAndOutletTempsAreSimilarAndHotAsync());
    }

    [TestMethod]
    public async Task TestSimilarTempsAtHighTempIsTrue()
    {
        Mock<IGasInletTemperatureSensors> sensorMock = new Mock<IGasInletTemperatureSensors>();

        sensorMock.Setup(mock => mock.GetFlowCelsiusAsync()).ReturnsAsync(82);
        sensorMock.Setup(mock => mock.GetReturnCelsiusAsync()).ReturnsAsync(84);

        var tester = new IndirectHeatingIsSaturatedTester(sensorMock.Object);

        Assert.IsFalse(await tester.GasHeatingInletAndOutletTempsAreSimilarAndHotAsync());
    }

    [TestMethod]
    public async Task TestDisimilarTempsAtHighTempIsFalse()
    {
        Mock<IGasInletTemperatureSensors> sensorMock = new Mock<IGasInletTemperatureSensors>();

        sensorMock.Setup(mock => mock.GetFlowCelsiusAsync()).ReturnsAsync(84);
        sensorMock.Setup(mock => mock.GetFlowCelsiusAsync()).ReturnsAsync(70);

        var tester = new IndirectHeatingIsSaturatedTester(sensorMock.Object);

        Assert.IsFalse(await tester.GasHeatingInletAndOutletTempsAreSimilarAndHotAsync());
    }

    [TestMethod]
    public async Task TestDisimilarTempsAtHighTempIsFalseWithBackwardsSensors()
    {
        Mock<IGasInletTemperatureSensors> sensorMock = new Mock<IGasInletTemperatureSensors>();

        sensorMock.Setup(mock => mock.GetFlowCelsiusAsync()).ReturnsAsync(70);
        sensorMock.Setup(mock => mock.GetFlowCelsiusAsync()).ReturnsAsync(84);

        var tester = new IndirectHeatingIsSaturatedTester(sensorMock.Object);

        Assert.IsFalse(await tester.GasHeatingInletAndOutletTempsAreSimilarAndHotAsync());
    }
}