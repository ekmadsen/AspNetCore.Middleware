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
* UseErikTheCoderClientPackages() enables mapping a URL to a physical location on disk.  This allows you to customize the URLs used by Javascript client packages.  For example using /clientpackages/bootstrap instead of /node_modules/bootstrap.
* UseErikTheCoderLogging() enables automatic logging of page and service method invocations.  It also automatically retrieves the correlation ID from HTTP headers so logs across application layers are related by a GUID, or if not found, it inserts a new GUID into the HTTP headers.  See my [Service Proxy](https://github.com/ekmadsen/ServiceProxy) solution, which, among other features, automatically inserts a GUID correlation ID into the HTTP requests sent by Refit service proxies.
* UseErikTheCoderExceptionHandling() enables automatic logging of all uncaught page and service method exceptions.  It responds to the caller with exception details formatted as JSON (for services) or HTML (for websites). This enables exception details to flow from a SQL database through a service to a website, displaying a full cross-process stack trace (related by CorrelationId) in the web browser. This greatly reduces the time it takes for a programmer to identify the root cause of application exceptions.
* AddErikTheCoderAuthentication enables custom authentication tokens that are mapped to a given user account with given roles and claims.  This allows a client to authenticate to a service by sending a simple "Authorization: ErikTheCoder Token" HTTP header, where "Token" is replaced by a secret string configured on the client and the service (typically in the project's appSettings.json file). The service constructs a ClaimsPrincipal with the given identity, attaches the given roles and claims, and updates the User object on the current HttpContext.Request.  Authorization proceeds normally in the ASP.NET Core pipeline, with policies examining this updated User identity to decide whether to grant access or not.  This is a convenient way to authenticate older clients or otherwise limited clients, such as the SharePoint workflow engine.
* UseErikTheCoderPolicies implements a couple trivial policies: Admin and Everyone.  I intend to add more common policies here so I don't write them again and again in mulitiple ASP.NET Core projects.
 

# Limitations #


# Installation #


# Usage #


# Benefits #

