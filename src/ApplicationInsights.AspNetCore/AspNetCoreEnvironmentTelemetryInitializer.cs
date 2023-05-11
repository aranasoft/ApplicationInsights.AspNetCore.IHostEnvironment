using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Hosting;

namespace Aranasoft.ApplicationInsights.AspNetCore
{
    /// <summary>
    /// <see cref="ITelemetryInitializer"/> implementation that stamps ASP.NET Core environment name
    /// on telemetries.
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
    public class AspNetCoreEnvironmentTelemetryInitializer : ITelemetryInitializer
    {
        private const string AspNetCoreEnvironmentPropertyName = "AspNetCoreEnvironment";
        private readonly IHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreEnvironmentTelemetryInitializer"/> class.
        /// </summary>
        /// <param name="environment">HostingEnvironment to provide EnvironmentName to be added to telemetry properties.</param>
        public AspNetCoreEnvironmentTelemetryInitializer(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        /// <inheritdoc />
        public void Initialize(ITelemetry telemetry)
        {
            if (this.environment != null)
            {
                if (telemetry is ISupportProperties telProperties && !telProperties.Properties.ContainsKey(AspNetCoreEnvironmentPropertyName))
                {
                    telProperties.Properties.Add(
                        AspNetCoreEnvironmentPropertyName,
                        this.environment.EnvironmentName);
                }
            }
        }
    }
}
