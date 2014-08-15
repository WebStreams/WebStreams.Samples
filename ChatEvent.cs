// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Describes a chat event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebStreams.Sample
{
    using System;

    /// <summary>
    /// Describes a chat event.
    /// </summary>
    public class ChatEvent
    {
        /// <summary>
        /// Gets or sets the UTC date and time of the quote.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public string Type { get; set; }
    }
}
