using System.Reflection;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class ParallelPricer
    {
        private readonly ParallelOptions _parallelOptions;
        private readonly Lazy<Dictionary<string, IPricingEngine>> _pricers;

        public ParallelPricer(int maxDegreeOfParallelism = 0)
        {
            _parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1)
            };
            
            _pricers = new Lazy<Dictionary<string, IPricingEngine>>(
                PricerLoader.LoadPricers,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public void PriceSingleTrade(ITrade trade, IScalarResultReceiver resultReceiver)
        {
            PricerLoader.PriceSingleTrade(trade, resultReceiver, _pricers.Value);
        }

        public void Price(IEnumerable<IEnumerable<ITrade>> tradeContainers, IScalarResultReceiver resultReceiver)
        {
            var threadSafeReceiver = new ThreadSafeResultReceiver(resultReceiver);
            var pricers = _pricers.Value;
            
            var allTrades = tradeContainers.SelectMany(container => container).ToList();

            Parallel.ForEach(allTrades, _parallelOptions, trade => 
            {
                try
                {
                    PricerLoader.PriceSingleTrade(trade, threadSafeReceiver, pricers);
                    Console.WriteLine($"Completed parallel pricing for trade: {trade.TradeId}");
                }
                catch (Exception ex)
                {
                    threadSafeReceiver.AddError(trade.TradeId, $"Error pricing trade: {ex.Message}");
                }
            });
        }
        
        //wrapping the result receiver to make it thread safe
        private class ThreadSafeResultReceiver(IScalarResultReceiver inner) : IScalarResultReceiver
        {
            private readonly IScalarResultReceiver _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            private readonly object _lock = new();

            public void AddResult(string tradeId, double result)
            {
                lock (_lock)
                {
                    _inner.AddResult(tradeId, result);
                }
            }
            
            public void AddError(string tradeId, string error)
            {
                lock (_lock)
                {
                    _inner.AddError(tradeId, error);
                }
            }
        }
    }
}