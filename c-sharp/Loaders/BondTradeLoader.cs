using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class BondTradeLoader : AbstractTradeLoader
    {
        private const char Separator = ',';

        public override IEnumerable<ITrade> LoadTrades()
        {
            var tradeList = new BondTradeList();
            ProcessTradesStream(trade => tradeList.Add((BondTrade)trade));

            return tradeList;
        }

        protected override ITrade CreateTradeFromLine(string line_)
        {
            
            var items = line_.Split(new[] {Separator});
            var trade = new BondTrade(items[6], items[0]);
            trade.TradeDate = DateTime.Parse(items[1]);
            trade.Instrument = items[2];
            trade.Counterparty = items[3];
            trade.Notional = Double.Parse(items[4]);
            trade.Rate = Double.Parse(items[5]);
            //Console.WriteLine($"Created trade with type: {trade.TradeType}, date: {trade.TradeDate}, instrument: {trade.Instrument}, counterparty: {trade.Counterparty}, notional: {trade.Notional}, rate: {trade.Rate}, trade id: {trade.TradeId}");
            
            return trade;
        }

        protected override bool ShouldSkipLine(string? line, int lineCount)
        {
            return string.IsNullOrEmpty(line) || 
                   lineCount < 1;
        }
    }
}