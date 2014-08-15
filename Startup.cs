// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

[assembly: Microsoft.Owin.OwinStartup(typeof(WebStreams.Sample.Startup))]

namespace WebStreams.Sample
{
    using Dapr.WebStreams.Server;

    using Owin;

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
            app.UseWebStreams(new WebStreamsSettings { RoutePrefix = "/api/v1" });
        }
    }
}