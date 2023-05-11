using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Aranasoft.ApplicationInsights.AspNetCore
{
    /// <summary>
    /// <see cref="IConfigureOptions&lt;ApplicationInsightsServiceOptions&gt;"/> implementation that reads options from 'appsettings.json',
    /// environment variables and sets developer mode based on debugger state.
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
    internal class DefaultApplicationInsightsServiceConfigureOptions : IConfigureOptions<ApplicationInsightsServiceOptions>
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IConfiguration _userConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApplicationInsightsServiceConfigureOptions"/> class.
        /// </summary>
        /// <param name="environment"><see cref="IHostEnvironment"/> to use for retrieving ContentRootPath.</param>
        /// <param name="configuration"><see cref="IConfiguration"/>  from an application.</param>
        public DefaultApplicationInsightsServiceConfigureOptions(IHostEnvironment environment, IConfiguration configuration = null)
        {
            _hostEnvironment = environment;
            _userConfiguration = configuration;
        }

        /// <inheritdoc />
        public void Configure(ApplicationInsightsServiceOptions options)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(_hostEnvironment.ContentRootPath ?? Directory.GetCurrentDirectory());
            if (_userConfiguration != null)
            {
                configBuilder.AddConfiguration(_userConfiguration);
            }

            configBuilder.AddJsonFile("appsettings.json", true)
                .AddJsonFile(string.Format(CultureInfo.InvariantCulture, "appsettings.{0}.json", _hostEnvironment.EnvironmentName), true)
                .AddEnvironmentVariables();

            ApplicationInsightsExtensions.AddTelemetryConfiguration(configBuilder.Build(), options);

            if (Debugger.IsAttached)
            {
                options.DeveloperMode = true;
            }
        }
    }
}
