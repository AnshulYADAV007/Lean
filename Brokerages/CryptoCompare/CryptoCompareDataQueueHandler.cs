﻿using System;
using System.Collections.Generic;
using QuantConnect.Data;
using QuantConnect.Interfaces;
using QuantConnect.Packets;
using WebSocketSharp;
using Newtonsoft.Json;

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
            var soc = new WebSocket("wss://streamer.cryptocompare.com/socket.io/?EIO=2&transport=websocket");
            soc.OnOpen += (object sender, EventArgs e) => Console.WriteLine("Connection Opened : " + e.ToString());
            soc.OnMessage += (object sender, MessageEventArgs e) => Console.WriteLine("New message from controller: " + e.Data);
            soc.OnClose += (object sender, CloseEventArgs e) => Console.WriteLine("Connection Closed because: " + e.Reason);
            soc.Connect();
            soc.Send(MakePacket("init", new Dictionary<string, object>()
            {
                {"subs", new List<string> () {"0~Poloniex~BTC~USD"}}
            }
                               )
                    );
            Console.ReadKey(true);
        }

        private string MakePacket(string v, object p)
        {
            var result = new[] { v, p };

            return "42" + JsonConvert.SerializeObject(result);
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