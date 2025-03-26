using System;
using System.Reflection;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class PricerLoader
    {
        //loading pricers
        public static Dictionary<string, IPricingEngine> LoadPricers()
        {
            var pricers = new Dictionary<string, IPricingEngine>();
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PricingConfig", "PricingEngines.xml");
            var pricingConfigLoader = new PricingConfigLoader { ConfigFile = configPath };
            var pricerConfig = pricingConfigLoader.LoadConfig();

            // var pricingConfigLoader = new PricingConfigLoader { ConfigFile = @".\PricingConfig\PricingEngines.xml" };
            // var pricerConfig = pricingConfigLoader.LoadConfig();

            foreach (var configItem in pricerConfig)
            {
                string assemblyName = configItem.Assembly ?? throw new InvalidOperationException("Pricing engine assembly name not provided");
                string pricerType = configItem.TypeName ?? throw new InvalidOperationException("Pricing engine type name not provided");
                string tradeType = configItem.TradeType ?? throw new InvalidOperationException("Trade type not provided");

                try
                {
                    string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pricers.dll");
                    Assembly pricersAssembly = Assembly.LoadFrom(assemblyPath); 
                    Type? pricerClass = pricersAssembly.GetType(pricerType);
                    if (pricerClass == null)
                    {
                        throw new InvalidOperationException($"Pricing engine class {pricerType} could not be found.");
                    }

                    var pricerInstance = Activator.CreateInstance(pricerClass) as IPricingEngine;
                    if (pricerInstance == null)
                    {
                        throw new InvalidOperationException($"Failed to create an instance of {pricerType}.");
                    }
                    
                    pricers[tradeType] = pricerInstance;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading pricing engine for {tradeType}: {ex.Message}");
                }
            }
            
            return pricers;
        }
        
        //pricing a single trade
        public static void PriceSingleTrade(
            ITrade trade, 
            IScalarResultReceiver resultReceiver, 
            Dictionary<string, IPricingEngine> pricers)
        {
            if (!pricers.TryGetValue(trade.TradeType, out IPricingEngine? pricer))
            {
                resultReceiver.AddError(trade.TradeId, "No Pricing Engines available for this trade type");
                return;
            }
            
            pricer.Price(trade, resultReceiver);
        }
    }
}