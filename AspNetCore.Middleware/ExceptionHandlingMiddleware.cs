using System;
using System.IO;
using System.Threading.Tasks;
using ErikTheCoder.AspNetCore.Middleware.Options;
using ErikTheCoder.Logging;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public static class ExceptionHandlingMiddleware
    {
        [UsedImplicitly]
        public static void Enable(IApplicationBuilder ApplicationBuilder, ExceptionHandlingOptions Options)
        {
            if (Options.ResponseHandler != null)
            {
                if (Options.ExceptionResponseFormat.HasValue || Options.IncludeDetails)
                {
                    throw new ArgumentException($"If setting an {nameof(Options.ResponseHandler)}, do not set an {nameof(Options.ExceptionResponseFormat)} or set {nameof(Options.IncludeDetails)} to true.");
                }
            }
            ApplicationBuilder.UseExceptionHandler(AlternatePipeline  =>
            {
                // Run terminates the middleware pipeline.
                AlternatePipeline.Run(async HttpContext =>
                {
                    // Get correlation ID.
                    Guid correlationId = CorrelationId.Get(HttpContext);
                    // Get exception.
                    IExceptionHandlerFeature exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
                    SimpleException innerException;
                    if (exceptionHandlerFeature.Error is ApiException apiException)
                    {
                        // Exception occurred when a Refit proxy called a service method.
                        // Deserialize exception from JSON response.
                        try
                        {
                            SimpleException refitException = apiException.GetContentAs<SimpleException>() ?? new SimpleException(apiException, correlationId, Options.AppName, Options.ProcessName);
                            innerException = new SimpleException(refitException, correlationId, Options.AppName, Options.ProcessName,
                                "An exception occurred when a Refit proxy called a service method.");
                        }
                        catch
                        {
                            // Ignore exception when deserializing JSON response.
                            innerException = new SimpleException(apiException, correlationId, Options.AppName, Options.ProcessName);
                            innerException = new SimpleException(innerException, correlationId, Options.AppName, Options.ProcessName,
                                $"Failed to deserialize exception from service method response.  Ensure the service's {nameof(ExceptionHandlingMiddleware)} is configured to use {nameof(ExceptionResponseFormat)}.{nameof(ExceptionResponseFormat.Json)}.");
                        }
                    }
                    else
                    {
                        innerException = new SimpleException(exceptionHandlerFeature.Error, correlationId, Options.AppName, Options.ProcessName);
                    }
                    // Log exception.
                    SimpleException exception = new SimpleException(innerException, correlationId, Options.AppName, Options.ProcessName,
                        $"{HttpContext.Request.Method} with {HttpContext.Request.ContentType ?? "unknown"} content type to {HttpContext.Request.Path} resulted in HTTP status code {HttpContext.Response.StatusCode}.");
                    ILogger logger = ApplicationBuilder.ApplicationServices.GetRequiredService<ILogger>();
                    logger.Log(correlationId, exception);
					logger.LogMetric(correlationId, HttpContext.Request.Path, "Critical Error", 1);
                    // Respond to caller.
                    if (Options.ResponseHandler != null)
                    {
                        // Respond with customer handler.
                        Options.ResponseHandler(HttpContext, exception);
                        return;
                    }
                    // Respond with HTML or JSON.
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (Options.ExceptionResponseFormat)
                    {
                        case ExceptionResponseFormat.Html:
                            await RespondWithHtml(HttpContext, exception, Options.IncludeDetails);
                            break;
                        case ExceptionResponseFormat.Json:
                            await RespondWithJson(HttpContext, exception, Options.IncludeDetails);
                            break;
                        default:
                            throw new Exception($"{nameof(ExceptionResponseFormat)} {Options.ExceptionResponseFormat} not supported.");
                    }
                });
            });
        }


        private static async Task RespondWithHtml(HttpContext Context, SimpleException Exception, bool IncludeDetails)
        {
            SimpleException exception = Exception;
            if (!IncludeDetails)
            {
                // Remove inner exception that contains details.
                exception.InnerException = null;
            }
            using (StreamWriter streamWriter = new StreamWriter(Context.Response.Body))
            {
                await streamWriter.WriteLineAsync("<html><body style=\"font-family: Consolas, Courier New, Courier New\"; font-size: 10px;><pre>");
                await streamWriter.WriteLineAsync(exception.GetSummary(true, true));
                await streamWriter.WriteLineAsync("</pre></body></html>");
            }
        }


        private static async Task RespondWithJson(HttpContext Context, SimpleException Exception, bool IncludeDetails)
        {
            SimpleException exception = Exception;
            if (!IncludeDetails)
            {
                // Remove inner exception that contains details.
                exception.InnerException = null;
            }
            JObject serializableException = JObject.FromObject(exception);
            using (StreamWriter streamWriter = new StreamWriter(Context.Response.Body))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    await serializableException.WriteToAsync(jsonWriter);
                }
            }
        }
    }
}
