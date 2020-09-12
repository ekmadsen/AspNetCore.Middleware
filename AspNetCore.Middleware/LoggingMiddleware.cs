using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ErikTheCoder.AspNetCore.Middleware.Options;
using ErikTheCoder.Logging;
using ErikTheCoder.ServiceContract;
using ErikTheCoder.Utilities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public class LoggingMiddleware
    {
        private readonly ILogger _logger;
        private readonly LoggingOptions _options;
        private readonly RequestDelegate _next;
        private readonly IOptionsMonitor<CookieAuthenticationOptions> _cookieAuthenticationOptions;
        private readonly HashSet<string> _sensitivePhrases;


        public LoggingMiddleware(ILogger Logger, LoggingOptions Options, RequestDelegate Next, IOptionsMonitor<CookieAuthenticationOptions> CookieAuthenticationOptions = null)
        {
            _logger = Logger;
            _options = Options;
            _next = Next;
            _cookieAuthenticationOptions = CookieAuthenticationOptions;
            _sensitivePhrases = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase) { "password", "confirmpassword", "newpassword", "confirmnewpassword" };
        }


        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext Context)
        {
            if (_logger == null)
            {
                // Call next middleware component and return.
                await _next(Context);
                return;
            }
            try
            {
                await Log(Context);
            }
            catch (Exception exception)
            {
                _logger?.Log(Context.GetCorrelationId(), $"Exception occurred in {nameof(LoggingMiddleware)}.  {exception.GetSummary(true, true)}");
                throw;
            }
        }


        private async Task Log(HttpContext Context)
        {
            var requestPath = Context.Request.Path.ToString();
            if (_options.IgnoreUrls.Count > 0)
            {
                foreach (var ignoreUrl in _options.IgnoreUrls)
                {
                    var ignoreCanonicalUrl = ignoreUrl.StartsWith("/") ? ignoreUrl : $"/{ignoreUrl}";
                    if (requestPath.StartsWith(ignoreCanonicalUrl, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Ignore URL.
                        // Call next middleware component and return.
                        await _next(Context);
                        return;
                    }
                }
            }
            var correlationId = GetCorrelationId(Context);
            var truncatedPath = GetTruncatedPath(requestPath);
            // Log request, FBA cookie, and metrics.
            LogRequest(Context, correlationId, requestPath);
            if (_options.LogFbaCookie) LogFbaCookie(Context, correlationId);
            LogMetrics(Context, correlationId, truncatedPath);
            var stopwatch = Stopwatch.StartNew();
            // Call next middleware component.
            await _next(Context);
            stopwatch.Stop();
            // Log performance.
            _logger.LogPerformance(correlationId, truncatedPath, stopwatch.Elapsed);
        }


        private static Guid GetCorrelationId(HttpContext Context)
        {
            if (Context.Request.Headers.ContainsKey(CustomHttpHeader.CorrelationId))
            {
                return Guid.Parse(Context.Request.Headers[CustomHttpHeader.CorrelationId][0]);
            }
            // Create new correlation ID HTTP header.
            var correlationId = Guid.NewGuid();
            Context.Request.Headers[CustomHttpHeader.CorrelationId] = correlationId.ToString();
            return correlationId;
        }


        private string GetTruncatedPath(string RequestPath)
        {
            if (_options.TruncateUrls.Count > 0)
            {
                foreach (var truncateUrl in _options.TruncateUrls)
                {
                    var truncateCanonicalUrl = truncateUrl.StartsWith("/") ? truncateUrl : $"/{truncateUrl}";
                    if (RequestPath.StartsWith(truncateCanonicalUrl, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Truncate URL.
                        return truncateCanonicalUrl;
                    }
                }
            }
            return RequestPath;
        }


        private void LogRequest(HttpContext Context, Guid CorrelationId, string RequestPath)
        {
            _logger.Log(CorrelationId, $"Request path = {RequestPath}.");
            _logger.Log(CorrelationId, $"Request HTTP verb = {Context.Request.Method}.");
            _logger.Log(CorrelationId, $"Request content type = {Context.Request.ContentType}.");
            _logger.Log(CorrelationId, $"Request user = {Context.User.Identity.Name}.");
            // Avoid the expense of reading the request body unless necessary.
            if (_options.LogRequestParameters)
            {
                // Log header, query, and form parameters.
                // Avoid logging sensitive phrases.
                foreach (var (key, values) in Context.Request.Headers)
                {
                    if (_sensitivePhrases.Contains(key)) continue;
                    _logger.Log(CorrelationId, $"Header Key = {key}, Values = {string.Join(", ", values)}");
                }
                foreach (var (key, values) in Context.Request.Query)
                {
                    if (_sensitivePhrases.Contains(key)) continue;
                    _logger.Log(CorrelationId, $"Query Key = {key}, Values = {string.Join(", ", values)}");
                }
                if (Context.Request.HasFormContentType)
                {
                    foreach (var (key, values) in Context.Request.Form)
                    {
                        if (_sensitivePhrases.Contains(key)) continue;
                        _logger.Log(CorrelationId, $"Form Key = {key}, Values = {string.Join(", ", values)}");
                    }
                }
            }
        }


        private void LogMetrics(HttpContext Context, Guid CorrelationId, string TruncatedPath)
        {
            _logger.LogMetric(CorrelationId, TruncatedPath, "Page Hit", 1);
            _logger.LogMetric(CorrelationId, TruncatedPath, "Page Hit - User", Context.User.Identity.Name ?? "Anonymous");
            _logger.LogMetric(CorrelationId, TruncatedPath, "Page Hit - Remote IP", Context.Connection.RemoteIpAddress.ToString());
        }


        private void LogFbaCookie(HttpContext Context, Guid CorrelationId)
        {
            if (_cookieAuthenticationOptions == null) return;
            // See https://stackoverflow.com/questions/42842511/how-to-manually-decrypt-an-asp-net-core-authentication-cookie
            // Retrieve encrypted HTTP cookie.
            var cookieName = $".AspNetCore.{CookieAuthenticationDefaults.AuthenticationScheme}";
            var encryptedCookie = _cookieAuthenticationOptions.CurrentValue.CookieManager.GetRequestCookie(Context, cookieName);
            if (!string.IsNullOrEmpty(encryptedCookie))
            {
                var encryptedCookieBytes = Base64UrlTextEncoder.Decode(encryptedCookie);
                // Decrypt cookie and remove control characters.
                var dataProtector = _cookieAuthenticationOptions.CurrentValue.DataProtectionProvider.CreateProtector
                    ("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", CookieAuthenticationDefaults.AuthenticationScheme, "v2");
                string cookie;
                try
                {
                    var cookieBytes = dataProtector.Unprotect(encryptedCookieBytes);
                    cookie = Encoding.UTF8.GetString(cookieBytes).RemoveControlCharacters();
                }
                catch (CryptographicException exception)
                {
                    // Cookie is signed by expired key.  This may occur if data protection is not configured in web server startup.
                    // Without data protection configured to persist keys, the web server creates a new key every time it recycles the application pool.
                    // See https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-2.2
                    cookie = $"Failed to decrypt FBA cookie.  {exception.GetSummary(true, true)}";
                }
                _logger.Log(CorrelationId, $"FBA Cookie = {cookie}");

                // The above cookie string approximates the data passed from the user's web browser to this web server.
                // It's an approximation because it was created by a binary serializer that converted an AuthenticationTicket class to a byte array.
                // See https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.ticketserializer.serialize?view=aspnetcore-2.2.

                // To reconstruct the AuthenticationTicket, do the following:
                //   TicketDataFormat ticketDataFormat = new TicketDataFormat(dataProtector);
                //   AuthenticationTicket authenticationTicket = ticketDataFormat.Unprotect(encryptedCookie);
                // Note that attempting to serialize AuthenticationTicket to JSON (to write to a log file or database) using Newtonsoft Json.NET causes a
                //   "PlatformNotSupportedException: This instance contains state that cannot be serialized and deserialized on this platform" error.
                //   This error occurs even with PreserveReferencesHandling.All and IgnoreSerializableInterface.
                // See https://github.com/JamesNK/Newtonsoft.Json/issues/1713.
            }
        }
    }
}
