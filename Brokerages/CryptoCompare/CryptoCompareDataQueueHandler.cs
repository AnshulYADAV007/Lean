using System;
using System.Collections.Generic;
using QuantConnect.Data;
using QuantConnect.Interfaces;
using QuantConnect.Packets;

namespace QuantConnect.Brokerages.CryptoCompare
{
    /// <summary>
    /// Crypto compare data queue handler.
    /// </summary>
    public class CryptoCompareDataQueueHandler : IDataQueueHandler
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:QuantConnect.Brokerages.CryptoCompare.CryptoCompareDataQueueHandler"/> class.
        /// </summary>
        public CryptoCompareDataQueueHandler()
        {

        }

        /// <summary>
        /// Gets the next ticks.
        /// </summary>
        /// <returns>The next ticks.</returns>
        public IEnumerable<BaseData> GetNextTicks()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Subscribe the specified job and symbols.
        /// </summary>
        /// <param name="job">Job.</param>
        /// <param name="symbols">Symbols.</param>
        public void Subscribe(LiveNodePacket job, IEnumerable<Symbol> symbols)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unsubscribe the specified job and symbols.
        /// </summary>
        /// <param name="job">Job.</param>
        /// <param name="symbols">Symbols.</param>
        public void Unsubscribe(LiveNodePacket job, IEnumerable<Symbol> symbols)
        {
            throw new NotImplementedException();
        }
    }
}