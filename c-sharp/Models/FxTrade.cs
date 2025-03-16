using System;

namespace HmxLabs.TechTest.Models
{
    public class FxTrade : BaseTrade
    {
        public FxTrade(string tradeId_, string tradeType_, DateTime valueDate_)
        {
            if (string.IsNullOrWhiteSpace(tradeId_))
            {
                throw new ArgumentException("A valid non null, non empty trade ID must be provided");
            }
            if (string.IsNullOrWhiteSpace(tradeType_))
            {
                throw new ArgumentException("A valid non null, non empty trade type must be provided");
            }
            if (valueDate_ == default(DateTime))
            {
                throw new ArgumentException("A valid value date must be provided");
            }
            
            TradeId = tradeId_;
            TradeType = tradeType_;
            ValueDate = valueDate_;
        }
        public const string FxSpotTradeType = "FxSpot";
        public const string FxForwardTradeType = "FxFwd";

        public override string TradeType { get; }

        public DateTime ValueDate { get; set; }
    }
}
