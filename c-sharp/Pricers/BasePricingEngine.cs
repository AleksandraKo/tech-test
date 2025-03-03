﻿using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Pricers
{
    public abstract class BasePricingEngine : IPricingEngine
    {
        public void Price(ITrade trade_, IScalarResultReceiver resultReceiver_)
        {
            if (null == resultReceiver_)
            {
                throw new ArgumentNullException("resultReceiver_");
            }

            if (null == trade_)
            {
                throw new ArgumentNullException("trade_");
            }

            PriceTrade(trade_, resultReceiver_);
        }

        public bool IsTradeTypeSupported(string tradeType_)
        {
            return _supportedTypes.ContainsKey(tradeType_);
        }

        protected BasePricingEngine()
        {
            Delay = 5000;
        }

        protected  void AddSupportedTradeType(string tradeType_)
        {
            _supportedTypes.Add(tradeType_, 0);
        }

        protected int Delay { get; set; }

        protected virtual void PriceTrade(ITrade trade_, IScalarResultReceiver resultReceiver_)
        {
            if (!IsTradeTypeSupported(trade_.TradeType))
            {
                if (null == trade_.TradeId)
                    throw new ArgumentException("Trade does not have a valid ID");
                
                resultReceiver_.AddError(trade_.TradeId, "Trade type not supported");
                return;
            }

            Console.WriteLine("Started pricing trade: " + trade_.TradeId);
            Thread.Sleep(Delay);
            var result = CalculateResult();

            if (TradesToError.ContainsKey(trade_.TradeId!))
            {
                resultReceiver_.AddError(trade_.TradeId!, TradesToError[trade_.TradeId]);
            }
            else
            {
                resultReceiver_.AddResult(trade_.TradeId, result);
                if (TradesToWarn.ContainsKey(trade_.TradeId))
                {
                    resultReceiver_.AddError(trade_.TradeId, TradesToWarn[trade_.TradeId]);
                }
            }

            Console.WriteLine("Completed pricing trade: " + trade_.TradeId);
        }

        protected virtual double CalculateResult()
        {
            return _random.NextDouble()*100;
        }

        private readonly Dictionary<string, uint> _supportedTypes = new Dictionary<string, uint>();
        private readonly Random _random = new Random();
        private static readonly Dictionary<string, string> TradesToError = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> TradesToWarn = new Dictionary<string, string>();

        static BasePricingEngine()
        {
            TradesToError.Add("GOV006", "Undefined error in pricing");
            TradesToWarn.Add("FWD001", "Unable to calibrate model to value date");
        }
    }
}