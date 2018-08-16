/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Securities;
using QuantConnect;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// Demonstration of using an external custom datasource. LEAN Engine is incredibly flexible and allows you to define your own data source.
    /// This includes any data source which has a TIME and VALUE. These are the *only* requirements. To demonstrate this we're loading in "Bitcoin" data.
    /// </summary>
    /// <meta name="tag" content="using data" />
    /// <meta name="tag" content="custom data" />
    /// <meta name="tag" content="crypto" />
    public class CustomDataCoinAPIAlgorithm : QCAlgorithm
    {
        private Security eth;
        private BollingerBands bb;

        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        public override void Initialize()
        {
            //Weather data we have is within these days:
            SetStartDate(2018, 8, 8);
            SetEndDate(DateTime.Now.Date);

            //Set the cash for the strategy:
            SetCash(100000);

            //Define the symbol and "type" of our generic data:
            eth = AddData<Crypto>("ETHUSD", Resolution.Tick);
            var consolidator_alt = new BaseDataConsolidator(TimeSpan.FromMinutes(60));
            SubscriptionManager.AddConsolidator(eth.Symbol, consolidator_alt);
            consolidator_alt.DataConsolidated += EveryConsolidatedPeriodALT;

            bb = new BollingerBands(12, 1.5m, MovingAverageType.Simple);
            RegisterIndicator(eth.Symbol, bb, consolidator_alt);
        }

        public void EveryConsolidatedPeriodALT(object sender, TradeBar consolidated)
        {
            if (IsWarmingUp)
                return;

            if (consolidated.Close > bb.UpperBand)
                Console.WriteLine("SetHoldings(eth.Symbol, 0.5m);");

            if (consolidated.Close < bb.LowerBand)
                Console.WriteLine("SetHoldings(eth.Symbol, 0.5m);");
        }

        public override void OnData(Slice data)
        {
            Console.WriteLine(data.ToString());
        }

        /// <summary>
        /// Custom Data Type: Bitcoin data from Quandl - http://www.quandl.com/help/api-for-bitcoin-data
        /// </summary>
        public class Crypto : Tick
        {
            public Crypto()
            {

            }

            /// <summary>
            /// 1. DEFAULT CONSTRUCTOR: Custom data types need a default constructor.
            /// We search for a default constructor so please provide one here. It won't be used for data, just to generate the "Factory".
            /// </summary>
            public Crypto(DateTime time, Symbol symbol, decimal price)
                : base(time, symbol, price, price, price)
            {

            }

            /// <summary>
            /// 2. RETURN THE STRING URL SOURCE LOCATION FOR YOUR DATA:
            /// This is a powerful and dynamic select source file method. If you have a large dataset, 10+mb we recommend you break it into smaller files. E.g. One zip per year.
            /// We can accept raw text or ZIP files. We read the file extension to determine if it is a zip file.
            /// </summary>
            /// <param name="config">Configuration object</param>
            /// <param name="date">Date of this source file</param>
            /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
            /// <returns>String URL of source file.</returns>
            public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
            {
                Dictionary<string, object> hello_message = new Dictionary<string, object>();
                hello_message.Add("type", "hello");
                hello_message.Add("apikey", "36A7D135-FC0B-49D5-B987-5F86D37C3F17");
                hello_message.Add("heartbeat", false);
                hello_message.Add("subscribe_data_type", new List<string>() { "trade" });
                hello_message.Add("subscribe_filter_symbol_id", new List<string>() {
                        "BITFINEX_SPOT_ETH_USD",
                        "COINBASE_SPOT_ETH_USD"
                        });
                string msg = JsonConvert.SerializeObject(hello_message);
                return new SubscriptionDataSource("wss://ws.coinapi.io/v1/", SubscriptionTransportMedium.WebSocket, FileFormat.Csv, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(msg, "") });
            }

            /// <summary>
            /// 3. READER METHOD: Read 1 line from data source and convert it into Object.
            /// Each line of the CSV File is presented in here. The backend downloads your file, loads it into memory and then line by line
            /// feeds it into your algorithm
            /// </summary>
            /// <param name="line">string line from the data source file submitted above</param>
            /// <param name="config">Subscription data, symbol name, data type</param>
            /// <param name="date">Current date we're requesting. This allows you to break up the data source into daily files.</param>
            /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
            /// <returns>New Bitcoin Object which extends BaseData.</returns>
            public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
            {
                if (line == null)
                {
                    return null;
                }
                var lineData = JObject.Parse(line);
                var price = lineData.Value<decimal>("price");
                Crypto coin = new Crypto(lineData.Value<DateTime>("time_exchange").AddDays(-4), "ETHUSD", price);
                //Console.WriteLine(lineData.ToString());
                return coin;
            }
    }
}
}