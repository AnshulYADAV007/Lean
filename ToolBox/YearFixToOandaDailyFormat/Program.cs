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
using System.IO;
using System.Linq;
using QuantConnect.Data.Market;

namespace QuantConnect.ToolBox.YearFixToOandaDailyFormat
{
    class Program
    {
        /// <summary>
        /// Supports data from http://TrueData.in
        /// </summary>
        public static void Main(string[] args)
        {
            //Document the process:
            Console.WriteLine("QuantConnect.ToolBox: NseMarketData Converter: ");
            Console.WriteLine("==============================================");
            Console.WriteLine("The NseMarketData converter transforms NseMarketData orders into the LEAN Algorithmic Trading Engine Data Format.");
            Console.WriteLine("Two parameters are required: ");
            Console.WriteLine("   1> Source Directory of Unzipped NSE Data.");
            Console.WriteLine("   2> Destination Directory of LEAN Data Folder. (Typically located under Lean/Data)");
            Console.WriteLine(" ");
            Console.WriteLine("NOTE: THIS WILL OVERWRITE ANY EXISTING FILES.");
            if (args.Length == 0)
            {
                Console.WriteLine("Press any key to Continue or Escape to quit.");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
            }
            string destinationDirectory;
            string sourceDirectory;
            if (args.Length == 2)
            {
                sourceDirectory = args[0];
                destinationDirectory = args[1];
            }
            else
            {
                Console.WriteLine("1. Source NSE source directory: ");
                sourceDirectory = (Console.ReadLine() ?? "");
                Console.WriteLine("2. Destination LEAN Data directory: ");
                destinationDirectory = (Console.ReadLine() ?? "");
            }

            //Validate the user input:
            Validate(sourceDirectory, destinationDirectory);

            //Remove the final slash to make the path building easier:
            sourceDirectory = StripFinalSlash(sourceDirectory);
            destinationDirectory = StripFinalSlash(destinationDirectory);

            //iterate over all the dates

            //Count the total files to process:
            Console.WriteLine("Counting Files..." + sourceDirectory);
            var totalCount = GetCount(sourceDirectory);
            Console.WriteLine("Processing {0} Files ...", totalCount);

            foreach (var file in Directory.EnumerateFiles(sourceDirectory))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string line;
                    var sym = Symbol.Create("USDZAR",SecurityType.Forex, Market.USA);
                    // Read the stream to a string, and write the string to the console.
                    IList<Tick> fileEnum = new List<Tick>();
                    var datawriter = new LeanDataWriter(Resolution.Tick, sym, destinationDirectory);
                    while ((line = sr.ReadLine())!= null)
                    {
                        var cHSplit = line.Split(' ');
                        var time =
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)cHSplit[1]
                                .ToDecimal());
                        int currentDay = time.Day ; 
                        do
                        {
                            var ch = line.Split(' ');
                            time =
                                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double) ch[1]
                                    .ToDecimal());
                            var quoteBar = new Tick(time, sym, ch[2].ToDecimal(), ch[3].ToDecimal());
                            fileEnum.Add(quoteBar);
                            line = sr.ReadLine();
                        } while (time.Day == currentDay && line != null);
                        Console.WriteLine(time.DayOfYear);
                        if (time.DayOfYear == 180)
                        {
                            datawriter.Write(fileEnum);
                            fileEnum.Clear();
                        }
                    }
                    datawriter.Write(fileEnum);
                }
            }
            Console.ReadKey();
        }


        /// <summary>
        /// Application error: display error and then stop conversion
        /// </summary>
        /// <param name="error">Error string</param>
        private static void Error(string error)
        {
            Console.WriteLine(error);
            Console.ReadKey();
            Environment.Exit(0);
        }

        /// <summary>
        /// Get the count of the files to process
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <returns></returns>
        private static int GetCount(string sourceDirectory)
        {
            return Directory.EnumerateFiles(sourceDirectory).Count();
        }

        /// <summary>
        /// Remove the final slash to make path building easier
        /// </summary>
        private static string StripFinalSlash(string directory)
        {
            return directory.Trim('/', '\\');
        }

        /// <summary>
        /// Validate the users input and throw error if not valid
        /// </summary>
        private static void Validate(string sourceDirectory, string destinationDirectory)
        {
            if (string.IsNullOrWhiteSpace(sourceDirectory))
            {
                Error("Error: Please enter a valid source directory.");
            }
            if (string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Error("Error: Please enter a valid destination directory.");
            }
            if (!Directory.Exists(sourceDirectory))
            {
                Error("Error: Source directory does not exist.");
            }
            if (!Directory.Exists(destinationDirectory))
            {
                Error("Error: Destination directory does not exist.");
            }
        }
    }
}
