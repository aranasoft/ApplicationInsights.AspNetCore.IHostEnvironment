using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aranasoft.ApplicationInsights.AspNetCore
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> that allow adding Application Insights services to application.
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
    public static class ApplicationInsightsExtensions
    {
        private const string VersionKeyFromConfig = "version";
        private const string InstrumentationKeyFromConfig = "ApplicationInsights:InstrumentationKey";
        private const string ConnectionStringFromConfig = "ApplicationInsights:ConnectionString";
        private const string DeveloperModeFromConfig = "ApplicationInsights:TelemetryChannel:DeveloperMode";
        private const string EndpointAddressFromConfig = "ApplicationInsights:TelemetryChannel:EndpointAddress";

        private const string InstrumentationKeyForWebSites = "APPINSIGHTS_INSTRUMENTATIONKEY";
        private const string ConnectionStringEnvironmentVariable = "APPLICATIONINSIGHTS_CONNECTION_STRING";
        private const string DeveloperModeForWebSites = "APPINSIGHTS_DEVELOPER_MODE";
        private const string EndpointAddressForWebSites = "APPINSIGHTS_ENDPOINTADDRESS";

        private const string ApplicationInsightsSectionFromConfig = "ApplicationInsights";
        private const string TelemetryChannelSectionFromConfig = "ApplicationInsights:TelemetryChannel";

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used in NetStandard2.0 build.")]
        private const string EventSourceNameForSystemRuntime = "System.Runtime";

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used in NetStandard2.0 build.")]
        private const string EventSourceNameForAspNetCoreHosting = "Microsoft.AspNetCore.Hosting";

        /// <summary>
        /// Read configuration from appSettings.json, appsettings.{env.EnvironmentName}.json,
        /// IConfiguation used in an application and EnvironmentVariables.
        /// Bind configuration to ApplicationInsightsServiceOptions.
        /// Values can also be read from environment variables to support azure web sites configuration.
        /// </summary>
        /// <param name="config">Configuration to read variables from.</param>
        /// <param name="serviceOptions">Telemetry configuration to populate.</param>
        internal static void AddTelemetryConfiguration(
            IConfiguration config,
            ApplicationInsightsServiceOptions serviceOptions)
        {
            try
            {
                config.GetSection(ApplicationInsightsSectionFromConfig).Bind(serviceOptions);
                config.GetSection(TelemetryChannelSectionFromConfig).Bind(serviceOptions);

                if (config.TryGetValue(primaryKey: ConnectionStringEnvironmentVariable, backupKey: ConnectionStringFromConfig, value: out string connectionStringValue))
                {
                    serviceOptions.ConnectionString = connectionStringValue;
                }

                if (config.TryGetValue(primaryKey: InstrumentationKeyForWebSites, backupKey: InstrumentationKeyFromConfig, value: out string instrumentationKey))
                {
#pragma warning disable CS0618
                    serviceOptions.InstrumentationKey = instrumentationKey;
#pragma warning restore CS0618
                }

                if (config.TryGetValue(primaryKey: DeveloperModeForWebSites, backupKey: DeveloperModeFromConfig, value: out string developerModeValue))
                {
                    if (bool.TryParse(developerModeValue, out bool developerMode))
                    {
                        serviceOptions.DeveloperMode = developerMode;
                    }
                }

                if (config.TryGetValue(primaryKey: EndpointAddressForWebSites, backupKey: EndpointAddressFromConfig, value: out string endpointAddress))
                {
                    serviceOptions.EndpointAddress = endpointAddress;
                }

                if (config.TryGetValue(primaryKey: VersionKeyFromConfig, value: out string version))
                {
                    serviceOptions.ApplicationVersion = version;
                }
            }
            catch (Exception ex)
            {
                AppInsightsEventSource.Current.LogError(ex.ToInvariantString());
            }
        }

        private static bool TryGetValue(this IConfiguration config, string primaryKey, out string value, string backupKey = null)
        {
            value = config[primaryKey];

            if (backupKey != null && string.IsNullOrWhiteSpace(value))
            {
                value = config[backupKey];
            }

            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
