// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Dapr Labs">
//   Copyright 2014, Dapr Labs Pty. Ltd.
// </copyright>
// <summary>
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


[assembly: Microsoft.Owin.OwinStartup(typeof(WebStreamSampleHost.Startup))]

namespace WebStreamSampleHost
{
    using Owin;

    using WebStream;

    using WebStreamSample;

    /// <summary>
    /// Initialization routines.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configure the application.
        /// </summary>
        /// <param name="app">The app being configured.</param>
        public void Configuration(IAppBuilder app)
        {
            app.UseWebStream<ChatRoomController>();
            app.UseWebStream<StockTickerController>();
        }
    }
}