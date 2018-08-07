using System;
using NUnit.Framework;
using QuantConnect.Brokerages.CoinAPI;

namespace QuantConnect.Tests.Brokerages.CoinAPI
{
    [TestFixture]
    public class CoinAPIDataQueueHandlerTests
    {
        private CoinAPIDataQueueHandler queue;
        [Test]
        public void TestGetNextTick()
        {
            try
            {
                queue = new CoinAPIDataQueueHandler();
                Assert.IsNotNull(queue);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void TestHellooCoinAPI()
        {
            try
            {
                queue.sendHello();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}