using QuantConnect.Interfaces;
using System;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantConnect.Logging;

namespace QuantConnect.Lean.Engine.DataFeeds.Transport
{
    class WebSocketSubscriptionStreamReader : IStreamReader
    {
        private bool _isConnected = false;
        private WebSocket socket;
        private Queue<string> messages = new Queue<string>();

        public SubscriptionTransportMedium TransportMedium
        {
            get
            {
                return SubscriptionTransportMedium.WebSocket;
            }
        }

        public WebSocketSubscriptionStreamReader(string source, IReadOnlyList<KeyValuePair<string,string>> header = null, bool isLiveMode = false)
        {
            socket = new WebSocket(source);
            socket.OnOpen += (object sender, EventArgs e) =>
            {
                _isConnected = true;
                //Log.Trace("Connection Opened : " + e);
            };
            socket.OnMessage += (object sender, MessageEventArgs e) =>
            {
                //Log.Trace("Incoming Message : " + e.Data);
                messages.Enqueue(e.Data);
            };
            socket.OnClose += (object sender, CloseEventArgs e) => Log.Trace("Connection Closed because: " + e.Reason);
            socket.OnError += (object sender, ErrorEventArgs e) => Log.Trace("Connection Error because: " + JsonConvert.DeserializeObject(e.Message).ToString());
            socket.Connect();
            if (header != null)
            {
                socket.Send(header[0].Key);
            }
        }

        public bool EndOfStream
        {
            get
            {
                return !_isConnected;
            }
        }

        public void Dispose()
        {
            socket.Close();
            _isConnected = false;
        }

        public string ReadLine()
        {
            return messages.Count != 0 ? messages.Dequeue() :null;
        }
    }
}
