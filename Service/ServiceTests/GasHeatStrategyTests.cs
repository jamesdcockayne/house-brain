using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Service.Gas;

namespace ServiceTests;

[TestClass]
public class GasHeatStrategyTests
{
    [TestMethod]
    public async Task TestThatNoCallForHeatIfTankIsAlreadyAtTemp()
    {
        var relayMock = new Mock<IGasCallForHeatRelay>();

        var args = new List<bool>();

        relayMock.SetupSet(mock => mock.CallForHeat).Callback((bool enabled) => args.Add(enabled));
        relayMock.SetupGet(mock => mock.CallForHeat).Returns(() => args.LastOrDefault(defaultValue: false));

        var sensorMock = new Mock<ICylinderTemperatureSensor>();

        sensorMock.SetupGet(mock => mock.SensorCelsius0).Returns(90);

        var waiterMock = new Mock<IWait>();

        var gasHeat = new GasHeatStrategy(relayMock.Object, sensorMock.Object, waiterMock.Object);

        using CancellationTokenSource tokenSource = new CancellationTokenSource();

        await gasHeat.GasHeatAsync(targetTemp: 50, tokenSource.Token);

        CollectionAssert.AreEqual(expected: args, actual: new[] { false });
    }

    [TestMethod]
    public async Task TestCallForHeatIsUnsetIfTaskIsCancelled()
    {
        var relayMock = new Mock<IGasCallForHeatRelay>();

        var args = new List<bool>();

        relayMock.SetupSet(mock => mock.CallForHeat).Callback((bool enabled) => args.Add(enabled));
        relayMock.SetupGet(mock => mock.CallForHeat).Returns(() => args.LastOrDefault(defaultValue: false));

        var sensorMock = new Mock<ICylinderTemperatureSensor>();

        sensorMock.SetupGet(mock => mock.SensorCelsius0).Returns(30);

        var waiterMock = new Mock<IWait>();

        waiterMock.Setup(waiter => waiter.ShortWait()).Returns(async () => await Task.Delay(100));

        var gasHeat = new GasHeatStrategy(relayMock.Object, sensorMock.Object, waiterMock.Object);

        using CancellationTokenSource tokenSource = new CancellationTokenSource();

        Task gasHeatTask = gasHeat.GasHeatAsync(targetTemp: 50, tokenSource.Token);

        tokenSource.Cancel();

        await gasHeatTask;

        CollectionAssert.AreEqual(expected: args, actual: new[] { true, false });
    }

    [TestMethod]
    public async Task TestCallForHeatIsUnsetOnceTheTankReachesTemp()
    {
        var relayMock = new Mock<IGasCallForHeatRelay>();

        var args = new List<bool>();

        relayMock.SetupSet(mock => mock.CallForHeat).Callback((bool enabled) => args.Add(enabled));
        relayMock.SetupGet(mock => mock.CallForHeat).Returns(() => args.LastOrDefault(defaultValue: false));

        var sensorMock = new Mock<ICylinderTemperatureSensor>();

        sensorMock.SetupGet(mock => mock.SensorCelsius0).Returns(30);

        var waiterMock = new Mock<IWait>();

        waiterMock.Setup(waiter => waiter.ShortWait()).Returns(async () => await Task.Delay(100));

        var gasHeat = new GasHeatStrategy(relayMock.Object, sensorMock.Object, waiterMock.Object);

        using CancellationTokenSource tokenSource = new CancellationTokenSource();

        Task gasHeatTask = gasHeat.GasHeatAsync(targetTemp: 50, tokenSource.Token);

        await Task.Delay(100); // Wait for GasHeatAsync to call for heat;

        // So far we should just have a call for heat and the task should still be running because the tank is not warm enough.
        //Assert.AreEqual(TaskStatus.Running, gasHeatTask.Status); 

        CollectionAssert.AreEqual(expected: args, actual: new[] { true });

        // Now make the tank hot, and wait for the task to finish. The call for heat should be unset.
        sensorMock.SetupGet(mock => mock.SensorCelsius0).Returns(90);

        await gasHeatTask;

        CollectionAssert.AreEqual(expected: args, actual: new[] { true, false });
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task TestExceptionIsThrownIfCallForHeatWasAlreadyInPlace()
    {
        var relayMock = new Mock<IGasCallForHeatRelay>();

        var args = new List<bool>();

        relayMock.SetupGet(mock => mock.CallForHeat).Returns(true);

        var sensorMock = new Mock<ICylinderTemperatureSensor>();
        var waiterMock = new Mock<IWait>();

 
        var gasHeat = new GasHeatStrategy(relayMock.Object, sensorMock.Object, waiterMock.Object);

        using CancellationTokenSource tokenSource = new CancellationTokenSource();

        await gasHeat.GasHeatAsync(targetTemp: 50, tokenSource.Token);
    }
}