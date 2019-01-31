# AspNetCore.Middleware
Middleware injected into ASP.NET Core pipeline at startup

# Motivation #

I was motived to write my own middleware components for the following reasons.

1. I wanted to integrate my [Logging](https://github.com/ekmadsen/Logging) component into middleware that *automatically* logs page and service method invocations and exceptions.  I wanted to remove the burden from developers to insert try / catch / finally logging code into every controller action.  If exceptions are uncaught by controller actions, I want the middleware to intercept them, log them, and format the response for the client to enable exception details to pass through application layers.
2. I wanted to place the boilerplate code that implements features commonly required by websites and services in a single solution, compile and deploy the solution as a [NuGet package](https://www.nuget.org/packages/ErikTheCoder.AspNetCore.Middleware/), and reference it in all my ASP.NET Core website and service projects.  This avoids duplicating the common features in the source code of every ASP.NET Core website and service project.
3. I needed to overcome a rediculous limitation in SharePoint workflow, where workflow variables cannot exceed 255 characters.  Because JWT security tokens are longer than 255 characters, I could not authenticate to custom REST / JSON services I had written using ASP.NET Core Web API.  My [AuthenticationHandler](https://github.com/ekmadsen/AspNetCore.Middleware/blob/master/AspNetCore.Middleware/AuthenticationHandler.cs) class overcomes this limitation by intercepting Authorization tokens prior to executing the default JWT authentication handler.
3. Regarding the "build versus buy versus download free component" decision, my reasons for writing my own ASP.NET Core middleware are the same as what I stated in points 3 - 6 in my [Logging ReadMe](https://github.com/ekmadsen/Logging/blob/master/README.md#motivation).

# Features #



# Limitations #


# Installation #


# Usage #


# Benefits #

