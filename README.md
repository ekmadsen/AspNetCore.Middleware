# AspNetCore.Middleware
Middleware injected into ASP.NET Core pipeline at startup

# Motivation #

I was motived to write my own middleware components for the following reasons.

1. I wanted to integrate my [Logging](https://github.com/ekmadsen/Logging) component into middleware that *automatically* logs page and service method invocations and exceptions.  I wanted to remove the burden from developers to insert try / catch / finally logging code into every controller action.  If exceptions are uncaught by controller actions, I want the middleware to intercept them, log them, and format a response for the client to enable exception details to pass through application layers.  What do I mean by *automatic*? The programmer need not include any boilerplate code in their controllers.
2. I wanted to place the boilerplate code that implements features commonly required by websites and services in a single solution, compile and deploy the solution as a [NuGet package](https://www.nuget.org/packages/ErikTheCoder.AspNetCore.Middleware/), and reference it in all my ASP.NET Core website and service projects.  This avoids duplicating the common features in the source code of every ASP.NET Core project.
3. I needed to overcome a ridiculous limitation in SharePoint workflow, where workflow variables cannot exceed 255 characters.  Because JWT security tokens are longer than 255 characters, I could not authenticate to custom REST / JSON services I had written using ASP.NET Core Web API.  My [AuthenticationHandler](https://github.com/ekmadsen/AspNetCore.Middleware/blob/master/AspNetCore.Middleware/AuthenticationHandler.cs) class overcomes this limitation by intercepting Authorization tokens prior to executing the default JWT authentication handler.
4. Regarding the "build versus buy versus download free component" decision, my reasons for writing my own ASP.NET Core middleware are the same as what I stated in points 3 - 6 in my [Logging ReadMe](https://github.com/ekmadsen/Logging/blob/master/README.md#motivation).

# Features #

You'll see me use the word *automatic* often here.  That's the point of this middleware: enable automatic features that relieve the programmer from manually implementing these features again and again, in every project, controller, and method.

* Targets .NET Standard 2.0 so it may be used in .NET Core or .NET Framework runtimes.
* Follows the .UseFeature() idiom recommended by Microsoft when configuring websites and services in the Startup class.
* UseErikTheCoderClientPackages() enables mapping a URL to a physical location on disk.  This allows you to customize the URLs used by Javascript client packages.  For example, using /clientpackages/bootstrap instead of /node_modules/bootstrap.
* UseErikTheCoderLogging() enables automatic logging of page and service method invocations (page hits and service method calls).  It also automatically retrieves the correlation ID from HTTP request headers so logs across application layers are related by a GUID, or if not found, it inserts a new GUID into the HTTP request headers.  See my [Service Proxy](https://github.com/ekmadsen/ServiceProxy) solution, which, among other features, automatically inserts a GUID correlation ID into the HTTP requests sent by Refit service proxies.
* UseErikTheCoderExceptionHandling() enables automatic logging of all uncaught page and service method exceptions.  It responds to the caller with exception details formatted as JSON (for services) or HTML (for websites). This enables exception details to flow from a SQL database through a service to a website, displaying a full cross-process stack trace (related by CorrelationId) in the web browser. This greatly reduces the time it takes for a programmer to identify the root cause of application exceptions.
* AddErikTheCoderAuthentication enables custom authentication tokens that are mapped to a given user account with given roles and claims.  This allows a client to authenticate to a service by sending a simple "Authorization: ErikTheCoder Token" HTTP header, where "Token" is replaced by a secret string configured on the client and the service (typically in the project's appSettings.json file). The service constructs a ClaimsPrincipal with the given identity, attaches the given roles and claims, and updates the User object on the current HttpContext.Request.  Authorization proceeds normally in the ASP.NET Core pipeline, with policies examining this updated User identity to decide whether to grant access or not.  This is a convenient way to authenticate older clients or otherwise limited clients, such as the SharePoint workflow engine.
* UseErikTheCoderPolicies implements a couple trivial policies: Admin and Everyone.  I intend to add more common policies here so I don't write them again and again in mulitiple ASP.NET Core projects.


# Installation #

Reference this component in your solution via its [NuGet package](https://www.nuget.org/packages/ErikTheCoder.AspNetCore.Middleware/).

# Usage #

In the Startup constructor, inject the HostingEnvironment dependency:

```C#
public class Startup
{
    private readonly IHostingEnvironment _hostingEnvironment;


    public Startup(IHostingEnvironment HostingEnvironment)
    {
        _hostingEnvironment = HostingEnvironment;
    }
```

In Startup.Configure, enable custom client package URL paths:

```C#
// Allow static files (css, js).
ApplicationBuilder.UseStaticFiles();
ApplicationBuilder.UseErikTheCoderClientPackages(Options =>
{
    Options.RequestUrlPath = clientPackagesPath;
    Options.FilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "node_modules");
});
```

In Startup.Configure, enable automatic logging, including ignoring or truncating certain URLs:

```C#
const string clientPackagesPath = "/clientpackages";
ApplicationBuilder.UseErikTheCoderLogging(Options =>
{
    Options.LogRequestParameters = Program.AppSettings.Logger.TraceLogLevel == LogLevel.Debug;
    Options.IgnoreUrls.Add(clientPackagesPath);
    Options.IgnoreUrls.Add("/css");
    Options.IgnoreUrls.Add("/images");
    Options.IgnoreUrls.Add("/js");
    Options.IgnoreUrls.Add("/favicon");
    Options.TruncateUrls.Add("/widget/display");
});
```

Truncation means to consider access to /Widget/Display/101 and /Widget/Display/102 as two page hits to the same /Widget/Display URL.  In other words, the last URL segment (the ID) is truncated from the URL path recorded in the logs.

In Startup.Configure, enable automatic exception handling.  This code redirects to an error page in a Production environment, which may display a user-friendly error message without any security-sensitive details.  In non-Production evironments, it displays exception details formatted as HTML:

```C#
// Configure exception handling.
if (_hostingEvnEnvironment.IsEnvironment(EnvironmentName.Prod))
{
    ApplicationBuilder.UseErikTheCoderExceptionHandling(Options =>
    {
        Options.AppName = Program.AppSettings.Logger.AppName;
        Options.ProcessName = Program.AppSettings.Logger.ProcessName;
        Options.ResponseHandler = (HttpContext, Exception) => HttpContext.Response.Redirect($"/error/display/{Exception.CorrelationId}");
    });
}
else
{
    ApplicationBuilder.UseBrowserLink();
    ApplicationBuilder.UseErikTheCoderExceptionHandling(Options =>
    {
        Options.AppName = Program.AppSettings.Logger.AppName;
        Options.ProcessName = Program.AppSettings.Logger.ProcessName;
        Options.ExceptionResponseFormat = ExceptionResponseFormat.Html;
        Options.IncludeDetails = true;
    });
}
```

For services, format exception details as JSON:

```C#
Options.ExceptionResponseFormat = ExceptionResponseFormat.Json;
```

In Startup.ConfigureServices, enable custom authentication tokens that fallback to JWT token authentication when no matching custom token is found:

```C#
// Require custom or JWT authentication token.
// The JWT token specifies the security algorithm used when it was signed (by Identity service).
Services.AddAuthentication(AuthenticationHandler.AuthenticationScheme).AddErikTheCoderAuthentication(Options =>
{
    Options.Identities = Program.AppSettings.AuthenticationIdentities;
    Options.ForwardDefaultSelector = HttpContext =>
    {
        // Forward to JWT authentication if custom token is not present.
        string token = string.Empty;
        if (HttpContext.Request.Headers.TryGetValue(AuthenticationHandler.HttpHeaderName, out StringValues authorizationValues)) token = authorizationValues.ToString();
        return token.StartsWith(AuthenticationHandler.TokenPrefix)
            ? AuthenticationHandler.AuthenticationScheme
            : JwtBearerDefaults.AuthenticationScheme;
    };
})
.AddJwtBearer(Options =>
{
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.AppSettings.CredentialSecret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(_clockSkewMinutes)
    };
});
```

In Startup.ConfigureServices, enable custom policies:

```C#
// Require authorization (permission to access controller actions) using custom claims.
Services.AddAuthorization(Options => Options.UseErikTheCoderPolicies());
```

# Benefits #

