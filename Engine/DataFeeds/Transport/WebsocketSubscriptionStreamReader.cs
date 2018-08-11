using QuantConnect.Interfaces;
using System;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine("Connection Opened : " + e);
            };
            socket.OnMessage += (object sender, MessageEventArgs e) =>
            {
                messages.Enqueue(e.Data);
                Console.WriteLine("New message from controller: " + e.Data);
            };
            socket.OnClose += (object sender, CloseEventArgs e) => Console.WriteLine("Connection Closed because: " + e.Reason);
            socket.OnError += (object sender, ErrorEventArgs e) => Console.WriteLine("Connection Error because: " + JsonConvert.DeserializeObject(e.Message).ToString());
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
            return messages.Dequeue();
        }
    }
}
