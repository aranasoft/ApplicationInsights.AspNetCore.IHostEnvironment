Microsoft Application Insights for ASP.NET Core applications
======
This code is a temporary workaround for the `IHostingEnvironment`/`IHostEnvironment` issue in the Application Insights SDK.  It is a temporary workaround until the SDK is updated to support the new `IHostEnvironment` interface.

The code is based on the [Microsoft Application Insights for ASP.NET Core applications](https://github.com/jayharris/ApplicationInsights-dotnet/tree/main/NETCORE).

The original issue is [Microsoft/ApplicationInsights-dotnet#2439](https://github.com/microsoft/ApplicationInsights-dotnet/issues/2439).

## Installation

From Package Manager Console:
```bash
PM> install-package Aranasoft.ApplicationInsights.AspNetCore.IHostEnvironment
```

## License

Copyright of Arana Software, released under the [BSD License](http://opensource.org/licenses/BSD-3-Clause/).
Copyright of Microsoft, released under the [MIT License](https://opensource.org/license/mit/).
