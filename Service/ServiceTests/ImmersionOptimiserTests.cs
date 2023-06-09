using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using Service.Immersion;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace ServiceTests
{
    [TestClass]
    public class ImmersionOptimiserTests
    {
        private static Mock<IWait> GetMockedWait()
        {
            var waiterMock = new Mock<IWait>();

            waiterMock.Setup(waiter => waiter.LongWaitAsync(CancellationToken.None)).Callback(() => { });

            return waiterMock;
        }

        [TestMethod]
        public async Task TopElementNoCurrent_BottomElementHasCurrent_TestSwicth()
        {
            var currentSensorMock = new Mock<ICurrentSensor>();

            currentSensorMock
                .SetupSequence(mock => mock.CurrentIsDetected)
                .Returns(false)
                .Returns(true);

            var relayMock = new Mock<IImmersionRelay>();

            var args = new List<bool>();

#pragma warning disable CS0618 // Type or member is obsolete
            relayMock.SetupSet(mock => mock.TopImmersionEnabled).Callback((bool enabled) => args.Add(enabled));
#pragma warning restore CS0618 // Type or member is obsolete
            relayMock.SetupGet(mock => mock.TopImmersionEnabled).Returns(() => args.LastOrDefault(defaultValue: true)); // TopImmersionEnabled should be false by default

            var waiterMock = GetMockedWait();

            var optimiser =
                new ImmersionOptimiser(
                    currentSensor: currentSensorMock.Object,
                    immersionRelay: relayMock.Object,
                    wait: waiterMock.Object,
                    logger: new FakeLogger<ImmersionOptimiser>());

            await optimiser.OptimiseAsync(CancellationToken.None);

            // We should have switched to the bottom relay and waited once.

            CollectionAssert.AreEqual(expected: args, actual: new[] { false });

            waiterMock.Verify(waiter => waiter.LongWaitAsync(CancellationToken.None), Times.Once());
        }

        [TestMethod]
        public async Task TopElementCurrent_TestRemainAtTopElement()
        {
            var currentSensorMock = new Mock<ICurrentSensor>();

            currentSensorMock
                .SetupGet(mock => mock.CurrentIsDetected)
                .Returns(true);

            var relayMock = new Mock<IImmersionRelay>();

            var args = new List<bool>();

#pragma warning disable CS0618 // Type or member is obsolete
            relayMock.SetupSet(mock => mock.TopImmersionEnabled).Callback((bool enabled) => args.Add(enabled));
#pragma warning restore CS0618 // Type or member is obsolete
            relayMock.SetupGet(mock => mock.TopImmersionEnabled).Returns(() => args.LastOrDefault(defaultValue: true)); // TopImmersionEnabled should be false by default

            var waiterMock = new Mock<IWait>();

            waiterMock.Setup(waiter => waiter.LongWaitAsync(CancellationToken.None)).Callback(() => { });

            var optimiser =
                new ImmersionOptimiser(
                    currentSensor: currentSensorMock.Object,
                    immersionRelay: relayMock.Object,
                    wait: waiterMock.Object,
                    logger: new FakeLogger<ImmersionOptimiser>());

            await optimiser.OptimiseAsync(CancellationToken.None);

            CollectionAssert.AreEqual(expected: args, actual: new bool[] { });

            waiterMock.Verify(waiter => waiter.LongWaitAsync(CancellationToken.None), Times.Once());
            
        }

        [TestMethod]
        public async Task BottomElementCurrentTopElementCurrent_TestSwapFromLowerToUpperElement()
        {
            // Test if we can pass current to the top element, if we can swap to the top element, else stay with the lower element.

            var currentSensorMock = new Mock<ICurrentSensor>();

            currentSensorMock
                .SetupGet(mock => mock.CurrentIsDetected)
                .Returns(true);

            var relayMock = new Mock<IImmersionRelay>();

            var args = new List<bool>();

#pragma warning disable CS0618 // Type or member is obsolete
            relayMock.SetupSet(mock => mock.TopImmersionEnabled).Callback((bool enabled) => args.Add(enabled));
#pragma warning restore CS0618 // Type or member is obsolete
            relayMock.SetupGet(mock => mock.TopImmersionEnabled).Returns(() => args.LastOrDefault(defaultValue: false)); // TopImmersionEnabled should be false by default

            var waiterMock = new Mock<IWait>();

            waiterMock.Setup(waiter => waiter.LongWaitAsync(CancellationToken.None)).Callback(() => { });

            var optimiser =
                new ImmersionOptimiser(
                    currentSensor: currentSensorMock.Object,
                    immersionRelay: relayMock.Object,
                    wait: waiterMock.Object,
                    logger: new FakeLogger<ImmersionOptimiser>());

            await optimiser.OptimiseAsync(CancellationToken.None);

            waiterMock.Verify(waiter => waiter.LongWaitAsync(CancellationToken.None), Times.Once());

            CollectionAssert.AreEqual(expected: args, actual: new[] { true });
        }

        [TestMethod]
        public async Task BottomElementCurrentTopElementNoCurrent_TestCheckTopElementThenReturnToLowerElement()
        {
            // Test if we can pass current to the top element, if we can swap to the top element, else stay with the lower element.

            var currentSensorMock = new Mock<ICurrentSensor>();

            currentSensorMock
                .SetupSequence(mock => mock.CurrentIsDetected)
                .Returns(true) // test the bottom element
                .Returns(false); // test the top element

            var relayMock = new Mock<IImmersionRelay>();

            var args = new List<bool>();

#pragma warning disable CS0618 // Type or member is obsolete
            relayMock.SetupSet(mock => mock.TopImmersionEnabled).Callback((bool enabled) => args.Add(enabled));
#pragma warning restore CS0618 // Type or member is obsolete
            relayMock.SetupGet(mock => mock.TopImmersionEnabled).Returns(() => args.LastOrDefault(defaultValue: false)); // TopImmersionEnabled should be false by default

            var waiterMock = new Mock<IWait>();

            waiterMock.Setup(waiter => waiter.LongWaitAsync(CancellationToken.None)).Callback(() => { });

            var optimiser =
                new ImmersionOptimiser(
                    currentSensor: currentSensorMock.Object,
                    immersionRelay: relayMock.Object,
                    wait: waiterMock.Object,
                    logger: new FakeLogger<ImmersionOptimiser>());

            await optimiser.OptimiseAsync(CancellationToken.None);
            
            waiterMock.Verify(waiter => waiter.LongWaitAsync(CancellationToken.None), Times.Once());

            CollectionAssert.AreEqual(expected: args, actual: new[] { true, false });
        }
    }
}