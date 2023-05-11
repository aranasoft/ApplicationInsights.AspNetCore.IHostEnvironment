using System.Diagnostics.Tracing;
using Microsoft.Extensions.Hosting;

namespace Aranasoft.ApplicationInsights.AspNetCore
{
    /// <remarks>
    /// This is a hack to get around the fact that Microsoft.ApplicationInsights.AspNetCore depends
    /// on IHostingEnvironment, which is not available in .NET Core 3.0+ and has been replaced
    /// with <see cref="IHostEnvironment"/>.
    ///
    /// This is a temporary workaround until the ApplicationInsights team updates their package to
    /// support .NET Core 3.0+ and <see cref="IHostEnvironment"/>.
    ///
    /// https://github.com/microsoft/ApplicationInsights-dotnet/issues/1869
    /// https://github.com/microsoft/ApplicationInsights-dotnet/issues/2439
    /// </remarks>
    [EventSource(Name = "YourFare-OrderConnect-ApplicationInsights")]
    internal sealed class AppInsightsEventSource : EventSource
    {
        public static readonly AppInsightsEventSource Current = new AppInsightsEventSource();
        private readonly ApplicationNameProvider applicationNameProvider = new ApplicationNameProvider();

        // Instance constructor is private to enforce singleton semantics
        private AppInsightsEventSource() : base() { }

        #region Keywords
        // Event keywords can be used to categorize events.
        // Each keyword is a bit flag. A single event can be associated with multiple keywords (via EventAttribute.Keywords property).
        // Keywords must be defined as a public class named 'Keywords' inside EventSource that uses them.
        public static class Keywords
        {
            public const EventKeywords Requests = (EventKeywords)0x1L;
            public const EventKeywords ServiceInitialization = (EventKeywords)0x2L;
            /// <summary>
            /// Keyword for errors that trace at Verbose level.
            /// </summary>
            public const EventKeywords Diagnostics = (EventKeywords)0x1;
        }
        #endregion

        #region Events
        // Define an instance method for each event you want to record and apply an [Event] attribute to it.
        // The method name is the name of the event.
        // Pass any parameters you want to record with the event (only primitive integer types, DateTime, Guid & string are allowed).
        // Each event method implementation should check whether the event source is enabled, and if it is, call WriteEvent() method to raise the event.
        // The number and types of arguments passed to every event method must exactly match what is passed to WriteEvent().
        // Put [NonEvent] attribute on all methods that do not define an event.
        // For more information see https://msdn.microsoft.com/en-us/library/system.diagnostics.tracing.eventsource.aspx

        /// <summary>
        /// Logs an event for when generic error occur within the Application Insights SDK.
        /// </summary>
        [Event(
            14,
            Keywords = Keywords.Diagnostics,
            Message = "An error has occurred which may prevent application insights from functioning. Error message: '{0}' ",
            Level = EventLevel.Error)]
        public void LogError(string errorMessage, string appDomainName = "Incorrect")
        {
            this.WriteEvent(14, errorMessage, this.applicationNameProvider.Name);
        }

        #endregion

        #region Private methods
#if UNSAFE
            private int SizeInBytes(string s)
            {
                if (s == null)
                {
                    return 0;
                }
                else
                {
                    return (s.Length + 1) * sizeof(char);
                }
            }
#endif
        #endregion
    }
}
