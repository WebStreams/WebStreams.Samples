// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Describes a stock at a point in time.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebStreams.Sample
{
    using System;

    /// <summary>
    /// Describes a stock at a point in time.
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// Gets or sets the UTC date and time of the quote.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public double Price { get; set; }
    }
}