using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class SerialPricer
    {
        //Lazy<T> for consistency
        private readonly Lazy<Dictionary<string, IPricingEngine>> _pricers;
        
        public SerialPricer()
        {
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
            var pricers = _pricers.Value;

            foreach (var tradeContainer in tradeContainers)
            {
                foreach (var trade in tradeContainer)
                {
                    PriceSingleTrade(trade, resultReceiver);
                }
            }
        }
    }
}