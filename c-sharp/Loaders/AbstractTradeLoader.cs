using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public abstract class AbstractTradeLoader : ITradeLoader, IStreamTradeLoader
    {
        public string? DataFile { get; set; }
        
        public abstract IEnumerable<ITrade> LoadTrades();

        public void ProcessTradesStream(Action<ITrade> tradeProcessor)
        {
            if (string.IsNullOrEmpty(DataFile))
                throw new ArgumentNullException(nameof(DataFile));
                
            try
            {
                using var reader = new StreamReader(DataFile);
                int lineCount = 0;

                while (!reader.EndOfStream)
                {
                    try
                    {
                        var line = reader.ReadLine();
                        if (ShouldSkipLine(line, lineCount))
                        {
                            lineCount++;
                            continue;
                        }

                        var trade = CreateTradeFromLine(line!);
                        tradeProcessor(trade);

                        lineCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing line {lineCount}: {ex.Message}");
                    }
                    lineCount++;
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new ApplicationException($"Trade data file not found: {DataFile}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ApplicationException($"Access denied to trade data file: {DataFile}", ex);
            }
            catch (IOException ex)
            {
                throw new ApplicationException($"I/O error reading trade data: {ex.Message}", ex);
            }
        }
        
        protected abstract ITrade CreateTradeFromLine(string line);
        
        protected virtual bool ShouldSkipLine(string? line, int lineCount)
        {
            return string.IsNullOrEmpty(line);
        }
    }
}