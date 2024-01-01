using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windwaker.Multiplayer.Client.Progression.Obtainables;

namespace Windwaker.Multiplayer.Client.Tests
{
    [TestClass]
    public class MemoryReaderTests
    {
        [TestMethod]
        public void TestItemIsObtained()
        {
            var item = new SingleItem("test", "test", 0x3000, 0x60, 0x3001);
            var notifier = new FakeNotifier();

            //item.CheckProgress(notifier, new TestReader());
            Assert.IsTrue(notifier.HasShown);
        }
    }
}