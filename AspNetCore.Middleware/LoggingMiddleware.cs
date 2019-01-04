using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ErikTheCoder.AspNetCore.Middleware.Options;
using ErikTheCoder.Logging;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public class LoggingMiddleware
    {
        private readonly ILogger _logger;
        private readonly LoggingOptions _options;
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _sensitivePhrases;


        public LoggingMiddleware(ILogger Logger, LoggingOptions Options, RequestDelegate Next)
        {
            _logger = Logger;
            _options = Options;
            _next = Next;
            _sensitivePhrases = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase) { "password" };
        }


        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext Context)
        {
            if (_logger is null)
            {
                // Call next middleware component and return.
                await _next(Context);
                return;
            }
            string requestPath = Context.Request.Path.ToString();
            if (_options.IgnoreUrls.Count > 0)
            {
                foreach (string ignoreUrl in _options.IgnoreUrls)
                {
                    string ignoreCanonicalUrl = ignoreUrl.StartsWith("/") ? ignoreUrl : $"/{ignoreUrl}";
                    if (requestPath.StartsWith(ignoreCanonicalUrl, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Ignore URL.
                        // Call next middleware component and return.
                        await _next(Context);
                        return;
                    }
                }
            }
            // Get correlation ID.
            Guid correlationId;
            if (Context.Request.Headers.ContainsKey(CustomHttpHeader.CorrelationId))
            {
                correlationId = Guid.Parse(Context.Request.Headers[CustomHttpHeader.CorrelationId][0]);
            }
            else
            {
                // Create new correlation ID HTTP header.
                correlationId = Guid.NewGuid();
                Context.Request.Headers[CustomHttpHeader.CorrelationId] = correlationId.ToString();
            }
            // Log request path and content type.
            _logger.Log(correlationId, $"Request path = {requestPath}.");
            _logger.Log(correlationId, $"Request HTTP verb = {Context.Request.Method}.");
            _logger.Log(correlationId, $"Request content type = {Context.Request.ContentType}.");
            _logger.Log(correlationId, $"Request user = {Context.User.Identity.Name}.");
            // Avoid the expense of reading the request body unless necessary.
            if (_options.LogRequestParameters)
            {
                // Log header, query, and form parameters.
                // Avoid logging sensitive phrases.
                foreach ((string key, StringValues values) in Context.Request.Headers)
                {
                    if (_sensitivePhrases.Contains(key)) continue;
                    _logger.Log(correlationId, $"Header Key = {key}, Values = {string.Join(", ", values)}");
                }
                foreach ((string key, StringValues values) in Context.Request.Query)
                {
                    if (_sensitivePhrases.Contains(key)) continue;
                    _logger.Log(correlationId, $"Query Key = {key}, Values = {string.Join(", ", values)}");
                }
                if (Context.Request.HasFormContentType)
                {
                    foreach ((string key, StringValues values) in Context.Request.Form)
                    {
                        if (_sensitivePhrases.Contains(key)) continue;
                        _logger.Log(correlationId, $"Form Key = {key}, Values = {string.Join(", ", values)}");
                    }
                }
            }
            // Log metrics.
            string truncatedPath = requestPath;
            if (_options.TruncateUrls.Count > 0)
            {
                foreach (string truncateUrl in _options.TruncateUrls)
                {
                    string truncateCanonicalUrl = truncateUrl.StartsWith("/") ? truncateUrl : $"/{truncateUrl}";
                    if (requestPath.StartsWith(truncateCanonicalUrl, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Truncate URL.
                        truncatedPath = truncateCanonicalUrl;
                        break;
                    }
                }
            }
            _logger.LogMetric(correlationId, truncatedPath, "Page Hit", 1);
            _logger.LogMetric(correlationId, truncatedPath, "Page Hit - User", Context.User.Identity.Name ?? "Anonymous");
            _logger.LogMetric(correlationId, truncatedPath, "Page Hit - Remote IP", Context.Connection.RemoteIpAddress.ToString());
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Call next middleware component.
            await _next(Context);
            stopwatch.Stop();
            // Log performance.
            _logger.LogPerformance(correlationId, truncatedPath, stopwatch.Elapsed);
        }
    }
}
