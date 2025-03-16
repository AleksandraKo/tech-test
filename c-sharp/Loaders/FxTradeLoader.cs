using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class FxTradeLoader : ITradeLoader
    {
        private const char Seperator = '¬';
        
        public IEnumerable<ITrade> LoadTrades()
        {
            var tradeList = new FxTradeList();
            LoadTradesFromFile(DataFile, tradeList);
            return tradeList;
        }

        public string? DataFile { get; set; }

        private FxTrade CreateTradeFromLine(string line_)
        {
            
            var items = line_.Split(new[] {Seperator});
            var trade = new FxTrade(items[8], items[0], DateTime.Parse(items[6]));
            trade.TradeDate = DateTime.Parse(items[1]);
            trade.Instrument = items[2] + items[3];
            trade.Notional = Double.Parse(items[4]);
            trade.Rate = Double.Parse(items[5]);
            trade.Counterparty = items[7];
            
            return trade;
        }

        private void LoadTradesFromFile(string? filename_, FxTradeList tradeList_)
        {
            if (null == filename_)
                throw new ArgumentNullException(nameof(filename_));
            
            var stream = new StreamReader(filename_);

            using (stream)
            {
                var lineCount = 0;
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    if (line == null)
                        continue;

                    if (line.StartsWith("END"))
                        break;

                    if (lineCount >= 2) {
                        tradeList_.Add(CreateTradeFromLine(line));
                    }
                    lineCount++;
                }
            }
        }
    }
}
