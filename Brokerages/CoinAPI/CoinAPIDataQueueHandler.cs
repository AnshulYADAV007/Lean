using System;
using System.Collections.Generic;
using QuantConnect.Data;
using QuantConnect.Interfaces;
using QuantConnect.Packets;
using WebSocketSharp;
using Newtonsoft.Json;

namespace QuantConnect.Brokerages.CoinAPI
{
    /// <summary>
    /// Crypto compare data queue handler.
    /// </summary>
    public class CoinAPIDataQueueHandler : IDataQueueHandler
    {
        private WebSocket socket;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:QuantConnect.Brokerages.CryptoCompare.CryptoCompareDataQueueHandler"/> class.
        /// </summary>
        public CoinAPIDataQueueHandler()
        {
            socket = new WebSocket("wss://ws.coinapi.io/v1/");
            socket.OnOpen += (object sender, EventArgs e) => Console.WriteLine("Connection Opened : " + e);
            socket.OnMessage += (object sender, MessageEventArgs e) => Console.WriteLine("New message from controller: " + e.Data);
            socket.OnClose += (object sender, CloseEventArgs e) => Console.WriteLine("Connection Closed because: " + e.Reason);
            socket.OnError += (object sender, ErrorEventArgs e) => Console.WriteLine("Connection Error because: " + JsonConvert.DeserializeObject(e.Message).ToString());
            socket.Connect();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Sends the hello. It uses Json strings to send objects to the server.
        /// </summary>
        public void sendHello()
        {
            Dictionary<string, object> hello_message = new Dictionary<string, object>();
            hello_message.Add("type", "hello");
            hello_message.Add("apikey", "");
            hello_message.Add("heartbeat", true);
            hello_message.Add("subscribe_data_type", new List<string>() { "trade" });
            hello_message.Add("subscribe_filter_symbol_id", new List<string>() {
                "BITSTAMP_SPOT_BTC_USD",
                "BITFINEX_SPOT_BTC_LTC",
                "COINBASE_",
                "ITBIT_" });
            string msg = JsonConvert.SerializeObject(hello_message);
            socket.Send(msg);
            System.Threading.Thread.Sleep(5000);
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