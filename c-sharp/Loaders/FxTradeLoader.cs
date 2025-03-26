using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class FxTradeLoader : AbstractTradeLoader
    {
        private const char Separator = '¬';
        
        public override IEnumerable<ITrade> LoadTrades()
        {
            var tradeList = new FxTradeList();
            
            ProcessTradesStream(trade => tradeList.Add((FxTrade)trade));
            
            return tradeList;
        }
        
        protected override ITrade CreateTradeFromLine(string line)
        {
            var items = line.Split(new[] {Separator});
            var trade = new FxTrade(items[8], items[0], DateTime.Parse(items[6]));
            trade.TradeDate = DateTime.Parse(items[1]);
            trade.Instrument = items[2] + items[3];
            trade.Notional = Double.Parse(items[4]);
            trade.Rate = Double.Parse(items[5]);
            trade.Counterparty = items[7];
            
            return trade;
        }
        
        protected override bool ShouldSkipLine(string? line, int lineCount)
        {
            return string.IsNullOrEmpty(line) || 
                   lineCount < 2 || 
                   line.StartsWith("END");
        }
    }
}