using HmxLabs.TechTest.Loaders;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class SerialTradeLoader
    {
        public IEnumerable<IEnumerable<ITrade>> LoadTrades()
        {
            var loaders = GetTradeLoaders();

            return loaders.Select(loader => loader.LoadTrades()).ToList();
        }

        // private IEnumerable<ITradeLoader> GetTradeLoaders()
        // {
        //     var loaders = new List<ITradeLoader>();
        //     ITradeLoader loader = new BondTradeLoader {DataFile = @"TradeData/BondTrades.dat"};
        //     loaders.Add(loader);

        //     loader = new FxTradeLoader { DataFile = @"TradeData/FxTrades.dat" };
        //     loaders.Add(loader);
            
        //     return loaders;
        // }
        private IEnumerable<ITradeLoader> GetTradeLoaders()
        {
            string tradeDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TradeData");

            var loaders = new List<ITradeLoader>();
            loaders.Add(new BondTradeLoader { DataFile = Path.Combine(tradeDataPath, "BondTrades.dat") });
            loaders.Add(new FxTradeLoader { DataFile = Path.Combine(tradeDataPath, "FxTrades.dat") });

            return loaders;
        }
    }
}