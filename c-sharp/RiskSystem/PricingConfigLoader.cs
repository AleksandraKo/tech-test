using System.Xml.Linq;

namespace HmxLabs.TechTest.RiskSystem
{
    public class PricingConfigLoader
    {
        public string? ConfigFile { get; set; }

        public PricingEngineConfig LoadConfig()
        {
            if (string.IsNullOrWhiteSpace(ConfigFile))
                throw new ArgumentException(nameof(ConfigFile));

            var configs = new PricingEngineConfig();
            var doc = XDocument.Load(ConfigFile);
            foreach (var engine in doc.Descendants("Engine"))
            {
                var configItem = new PricingEngineConfigItem
                {
                    TradeType = engine.Attribute("tradeType")?.Value,
                    Assembly = engine.Attribute("assembly")?.Value,
                    TypeName = engine.Attribute("pricingEngine")?.Value
                };

                configs.Add(configItem);
            }

            return configs;
        }
    }
}