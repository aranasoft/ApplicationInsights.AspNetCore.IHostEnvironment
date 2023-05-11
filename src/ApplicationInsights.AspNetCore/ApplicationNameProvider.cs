using System;
using Microsoft.Extensions.Hosting;

namespace Aranasoft.ApplicationInsights.AspNetCore
{
    /// <summary>
    /// This class provides the assembly name for the EventSource implementations.
    /// </summary>
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
    internal sealed class ApplicationNameProvider
    {
        public ApplicationNameProvider()
        {
            this.Name = GetApplicationName();
        }

        public string Name { get; private set; }

        private static string GetApplicationName()
        {
            //// We want to add application name to all events BUT
            //// It is prohibited by EventSource rules to have more parameters in WriteEvent that in event source method
            //// Parameter will be available in payload but in the next versions EventSource may
            //// start validating that number of parameters match
            //// It is not allowed to call additional methods, only WriteEvent

            string name;
            try
            {
                name = AppDomain.CurrentDomain.FriendlyName;
            }
            catch (Exception exp)
            {
                name = "Undefined " + exp.Message ?? exp.ToString();
            }

            return name;
        }
    }
}
