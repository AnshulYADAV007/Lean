using System;
using NUnit.Framework;
using QuantConnect.Brokerages.CryptoCompare;

namespace QuantConnect.Tests.Brokerages.CryptoCompare
{
    [TestFixture]
    public class CryptoCompareDataQueueHandlerTests
    {
        [Test]
        public void TestGetNextTick()
        {
            try
            {
                var queue = new CryptoCompareDataQueueHandler();
                Assert.IsNotNull(queue);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }
    }
}