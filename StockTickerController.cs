// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The stock ticker controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebStreams.Sample
{
    using System;
    using System.Collections.Concurrent;
    using System.Reactive.Linq;

    using Dapr.WebStreams.Server;

    /// <summary>
    /// The stock ticker controller.
    /// </summary>
    [RoutePrefix("/stock")]
    public class StockTickerController
    {
        /// <summary>
        /// The stocks.
        /// </summary>
        private readonly ConcurrentDictionary<string, IObservable<Stock>> stocks = new ConcurrentDictionary<string, IObservable<Stock>>();

        /// <summary>
        /// Returns the stock ticker for <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <returns>
        /// The stock ticker.
        /// </returns>
        [Route("ticker")]
        public IObservable<Stock> GetTicker(string symbol)
        {
            return this.GetStockTicker(symbol);
        }

        /// <summary>
        /// Creates and returns a new stock ticker.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <returns>
        /// A new stock symbol.
        /// </returns>
        private static IObservable<Stock> CreateStockTicker(string symbol)
        {
            var random = new Random();
            
            // Construct a pseudo-random walk.
            return
                Observable.Interval(TimeSpan.FromSeconds(0.1))
                    .Select(time => random.NextDouble())
                    .Scan((double)random.Next(1000), (prev, randomVal) => Math.Max(0, prev * (1 + (0.01 * (randomVal - 0.5)))))
                    .Select(val => new Stock { Symbol = symbol, Price = val, Time = DateTime.UtcNow }).Do(
                        val =>
                        {
                            Console.WriteLine(val);
                            Console.WriteLine(val);
                        });
        }

        /// <summary>
        /// Returns the stock ticker for <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <returns>
        /// The stock ticker.
        /// </returns>
        private IObservable<Stock> GetStockTicker(string symbol)
        {
            return this.stocks.GetOrAdd(symbol, CreateStockTicker);
        }
    }
}