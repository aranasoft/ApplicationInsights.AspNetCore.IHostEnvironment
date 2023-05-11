using System;
using Aranasoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AspNetCoreEnvironmentTelemetryInitializer = Aranasoft.ApplicationInsights.AspNetCore.AspNetCoreEnvironmentTelemetryInitializer;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
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
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Application Insights services into service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
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
        public static void AddApplicationInsightsTelemetryUsingHostEnvironment(this IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.RemoveAll<IConfigureOptions<ApplicationInsightsServiceOptions>>();
            services.TryAddSingleton<IConfigureOptions<ApplicationInsightsServiceOptions>, DefaultApplicationInsightsServiceConfigureOptions>();

            services.RemoveAll<ITelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, AzureAppServiceRoleNameFromHostNameHeaderInitializer>();
            services.AddSingleton<ITelemetryInitializer, ClientIpHeaderTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, OperationNameTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, SyntheticTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, WebSessionTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, WebUserTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, AspNetCoreEnvironmentTelemetryInitializer>();
        }

        /// <summary>
        /// Adds Application Insights services into service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="options">The action used to configure the options.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
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
        public static void AddApplicationInsightsTelemetryUsingHostEnvironment(this IServiceCollection services, Action<ApplicationInsightsServiceOptions> options)
        {
            services.AddApplicationInsightsTelemetryUsingHostEnvironment();
            services.Configure(options);
        }
    }
}