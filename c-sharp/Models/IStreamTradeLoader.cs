using System;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public interface IStreamTradeLoader
    {
        void ProcessTradesStream(Action<ITrade> tradeProcessor);
        string? DataFile { get; set; }
    }
}