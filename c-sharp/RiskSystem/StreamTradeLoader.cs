using HmxLabs.TechTest.Loaders;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class StreamTradeLoader
    {
        public void ProcessAllTrades(Action<ITrade> tradeProcessor)
        {
            var loaders = GetStreamingTradeLoaders();
            
            foreach (var loader in loaders)
            {
                loader.ProcessTradesStream(tradeProcessor);
            }
        }

        private IEnumerable<IStreamTradeLoader> GetStreamingTradeLoaders()
        {
            var loaders = new List<IStreamTradeLoader>();
            
            IStreamTradeLoader loader = new BondTradeLoader { DataFile = @"TradeData/BondTrades.dat" };
            loaders.Add(loader);

            loader = new FxTradeLoader { DataFile = @"TradeData/FxTrades.dat" };
            loaders.Add(loader);

            return loaders;
        }
    }
}