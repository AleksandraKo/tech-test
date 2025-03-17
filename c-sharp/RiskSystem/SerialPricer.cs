using System;
using System.Reflection;
using System.Collections.Generic;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class SerialPricer
    {
        public void Price(IEnumerable<IEnumerable<ITrade>> tradeContainters_, IScalarResultReceiver resultReceiver_)
        {
            LoadPricers();

            foreach (var tradeContainter in tradeContainters_)
            {
                foreach (var trade in tradeContainter)
                {
                    if (!_pricers.ContainsKey(trade.TradeType))
                    {
                        resultReceiver_.AddError(trade.TradeId, "No Pricing Engines available for this trade type");
                        continue;
                    }

                    var pricer = _pricers[trade.TradeType];
                    pricer.Price(trade, resultReceiver_);
                }
            }
        }

        private void LoadPricers()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PricingConfig", "PricingEngines.xml");
            var pricingConfigLoader = new PricingConfigLoader { ConfigFile = configPath };
            var pricerConfig = pricingConfigLoader.LoadConfig();

            foreach (var configItem in pricerConfig)
            {
                string assemblyName = configItem.Assembly ?? throw new InvalidOperationException("Pricing engine assembly name not provided");
                string pricerType = configItem.TypeName ?? throw new InvalidOperationException("Pricing engine type name not provided");
                string tradeType = configItem.TradeType ?? throw new InvalidOperationException("Trade type not provided");

                try
                {
                    string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pricers", "Pricers.dll");
                    assemblyPath = Path.GetFullPath(assemblyPath);
                    // Console.WriteLine($"Loading assembly from: {assemblyPath}");
                    // if (!File.Exists(assemblyPath))
                    // {
                    //     Console.WriteLine($"Error: Assembly file {assemblyPath} not found!");
                    //     continue;
                    // }
                    
                    Assembly pricersAssembly = Assembly.LoadFrom(assemblyPath); // Load the assembly dynamically
                    Type? pricerClass = pricersAssembly.GetType(pricerType); //reflection to dynamically load the pricer class type
                    if (pricerClass == null)
                    {
                        throw new InvalidOperationException($"Pricing engine class {pricerType} could not be found.");
                    }

                    // Create an instance of the pricing engine
                    var pricerInstance = Activator.CreateInstance(pricerClass) as IPricingEngine;
                    if (pricerInstance == null)
                    {
                        throw new InvalidOperationException($"Failed to create an instance of {pricerType}.");
                    }
                    
                    _pricers[tradeType] = pricerInstance;
                    // Console.WriteLine($"Successfully loaded pricer for {tradeType}: {pricerType}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading pricing engine for {tradeType}: {ex.Message}");
                }
            }
        }

        private readonly Dictionary<string, IPricingEngine> _pricers = new Dictionary<string, IPricingEngine>();
    }
}