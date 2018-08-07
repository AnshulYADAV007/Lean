﻿using System;
using NUnit.Framework;
using QuantConnect.Brokerages.CryptoCompare;

namespace QuantConnect.Tests.Brokerages.CryptoCompare
{
    [TestFixture]
    public class CryptoCompareDataQueueHandlerTests
    {
        private CryptoCompareDataQueueHandler queue;
        [Test]
        public void TestGetNextTick()
        {
            try
            {
                queue = new CryptoCompareDataQueueHandler();
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