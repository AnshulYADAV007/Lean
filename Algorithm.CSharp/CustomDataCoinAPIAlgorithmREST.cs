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
    public class CustomDataCoinAPIAlgorithmREST : QCAlgorithm
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
            eth = AddData<Crypto>("ETHUSD");
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
            Console.WriteLine("Following is the Data at (Time): " + data.Time);
            foreach (var dataitem in data)
            {
                Console.WriteLine(dataitem.ToString());
            }
        }

        /// <summary>
        /// Custom Data Type: Bitcoin data from Quandl - http://www.quandl.com/help/api-for-bitcoin-data
        /// </summary>
        public class Crypto : BaseData
        {
            /*
             *  [
                  {
                    "time_period_start": "2017-01-01T00:00:00.0000000Z",
                    "time_period_end": "2017-01-02T00:00:00.0000000Z",
                    "time_open": "2017-01-01T00:01:08.0000000Z",
                    "time_close": "2017-01-01T23:59:46.0000000Z",
                    "price_open": 966.340000000,
                    "price_high": 1005.000000000,
                    "price_low": 960.530000000,
                    "price_close": 997.750000000,
                    "volume_traded": 6850.593308590,
                    "trades_count": 7815
                  },
                  {
                    "time_period_start": "2017-01-02T00:00:00.0000000Z",
                    "time_period_end": "2017-01-03T00:00:00.0000000Z",
                    "time_open": "2017-01-02T00:00:05.0000000Z",
                    "time_close": "2017-01-02T23:59:37.0000000Z",
                    "price_open": 997.750000000,
                    "price_high": 1032.000000000,
                    "price_low": 990.010000000,
                    "price_close": 1012.540000000,
                    "volume_traded": 8167.381030180,
                    "trades_count": 7871
                  }
                ]
             *
             */
            [JsonProperty("time_period_start")]
            public DateTime TimePeriodStart;
            [JsonProperty("time_period_end")]
            public DateTime TimePeriodEnd;
            [JsonProperty("time_open")]
            public DateTime TimeOpen;
            [JsonProperty("time_close")]
            public DateTime TimeClose;
            [JsonProperty("price_open")]
            public decimal priceOpen;
            [JsonProperty("price_high")]
            public decimal priceHigh;
            [JsonProperty("price_low")]
            public decimal priceLow;
            [JsonProperty("price_close")]
            public decimal priceClose;
            [JsonProperty("volume_traded")]
            public decimal volumeTraded;
            [JsonProperty("trades_count")]
            public int tradesCount;


            private IEnumerable<Crypto> response;
            private bool firstRead = true;

            public Crypto()
            {

            }

            public override string ToString()
            {
                return string.Format("Time : {0} - {1} /n OHLC : {2} {3} {4} {5} Volume : {6} Count : {7}", TimeOpen, TimeClose, priceOpen, priceHigh, priceLow, priceClose, volumeTraded, tradesCount);
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
                /*      var client = new RestClient("https://rest.coinapi.io/v1/exchanges");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("X-CoinAPI-Key", "73034021-0EBC-493D-8A00-E0F138111F41");
                        IRestResponse response = client.Execute(request);*/
                return new SubscriptionDataSource("https://rest.coinapi.io/v1/ohlcv/BITSTAMP_SPOT_BTC_USD/history?period_id=1MIN&time_start=2016-01-01T00:00:00", SubscriptionTransportMedium.Rest, FileFormat.Csv, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("X-CoinAPI-Key", "36A7D135-FC0B-49D5-B987-5F86D37C3F17") });                
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
                if (firstRead)
                {
                    firstRead = false;
                    response = JsonConvert.DeserializeObject<List<Crypto>>(line);
                    foreach (var alt in response)
                    {
                        Console.WriteLine(alt.ToString());
                    }
                }
                return null;
            }
        }
    }
}