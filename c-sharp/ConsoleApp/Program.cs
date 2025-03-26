using HmxLabs.TechTest.Models;
using HmxLabs.TechTest.RiskSystem;
using System.Diagnostics;

namespace HmxLabs.TechTest.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args_)
        {
            //Parallel Approah
            Console.WriteLine("\nParallel Approach");
            ParallelApproach();
            
            //Stream Approach
            Console.WriteLine("\nStreamTradeLoader");
            StreamApproach();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void ParallelApproach()
        {
            var stopwatch = Stopwatch.StartNew();
            
            var results = new ScalarResults();
            var parallelPricer = new ParallelPricer();
            var tradeLoader = new SerialTradeLoader();
            var tradeContainers = tradeLoader.LoadTrades();
            
            parallelPricer.Price(tradeContainers, results);

            var printer = new ScreenResultPrinter();
            printer.PrintResults(results);
            
            stopwatch.Stop();
            Console.WriteLine($"Processing completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        private static void StreamApproach()
        {
            var stopwatch = Stopwatch.StartNew();

            var results = new ScalarResults();
            var pricer = new SerialPricer();
            var tradeLoader = new StreamTradeLoader();
            
            // Process each trade as it's loaded
            tradeLoader.ProcessAllTrades(trade => {
                pricer.PriceSingleTrade(trade, results);
            });

            var screenPrinter = new ScreenResultPrinter();
            screenPrinter.PrintResults(results);
            
            stopwatch.Stop();
            Console.WriteLine($"Processing completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        private static void OGApproach() 
        {
            var tradeLoader = new SerialTradeLoader();

            var allTrades = tradeLoader.LoadTrades();
            var results = new ScalarResults();
            var pricer = new SerialPricer();
            //var pricer = new ParallelPricer();
            pricer.Price(allTrades, results);

            var screenPrinter = new ScreenResultPrinter();
            screenPrinter.PrintResults(results);
        }
    }
}