using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class ScreenResultPrinter
    {
        public void PrintResults(ScalarResults results_)
        {
            foreach (var result in results_)
            {
                if (result.Result.HasValue && !string.IsNullOrWhiteSpace(result.Error))
                {
                    Console.WriteLine($"{result.TradeId} : {result.Result} : {result.Error}");
                }
                else if (result.Result.HasValue)
                {
                    Console.WriteLine($"{result.TradeId} : {result.Result}");
                }
                else if (!string.IsNullOrWhiteSpace(result.Error))
                {
                    Console.WriteLine($"{result.TradeId} : {result.Error}");
                }
            }
        }
    }
}