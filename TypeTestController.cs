// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The chat room controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebStreams.Sample
{
    using System;
    using System.Reactive.Linq;

    using Dapr.WebStreams.Server;

    /// <summary>
    /// The chat room controller.
    /// </summary>
    [RoutePrefix("/test")]
    public class TestController
    {
        /// <summary>
        /// Some silly test method.
        /// </summary>
        /// <param name="str">A string.</param>
        /// <param name="integer">An integer.</param>
        /// <param name="guid">A GUID.</param>
        /// <returns>A silly observable.</returns>
        [Route("test")]
        public IObservable<object> Test(string str, int integer, Guid guid)
        {
            return new object[] { str, integer, guid }.ToObservable();
        }

        /// <summary>
        /// Some silly test method.
        /// </summary>
        /// <param name="str">A string.</param>
        /// <param name="time">A DateTime.</param>
        /// <param name="guid">A GUID.</param>
        /// <returns>A silly observable.</returns>
        [Route("test2")]
        public IObservable<object> Test(string str, DateTime time, Guid guid)
        {
            return new object[] { str, time, guid }.ToObservable();
        }
    }
}