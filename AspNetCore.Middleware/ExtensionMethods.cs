﻿using System;
using ErikTheCoder.AspNetCore.Middleware.Options;
using ErikTheCoder.ServiceContract;
using ErikTheCoder.Utilities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AuthenticationOptions = ErikTheCoder.AspNetCore.Middleware.Options.AuthenticationOptions;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public static class ExtensionMethods
    {
        public static Guid GetCorrelationId(this HttpContext HttpContext)
        {
            return HttpContext.Request.Headers.TryGetValue(CustomHttpHeader.CorrelationId, out var correlationId)
                ? Guid.Parse(correlationId[0])
                : Guid.Empty;
        }


        [UsedImplicitly]
        public static void UseErikTheCoderClientPackages(this IApplicationBuilder ApplicationBuilder, Action<ClientPackagesOptions> ConfigureOptions = null)
        {
            var options = new ClientPackagesOptions();
            ConfigureOptions?.Invoke(options);
            if (options.RequestUrlPath.IsNullOrWhiteSpace()) throw new Exception($"{nameof(options.RequestUrlPath)} not specified.");
            if (options.FilePath.IsNullOrWhiteSpace()) throw new Exception($"{nameof(options.FilePath)} not specified.");
            ClientPackagesMiddleware.Enable(ApplicationBuilder, options);
        }


        [UsedImplicitly]
        public static void UseErikTheCoderLogging(this IApplicationBuilder ApplicationBuilder, Action<LoggingOptions> ConfigureOptions = null)
        {
            var options = new LoggingOptions();
            ConfigureOptions?.Invoke(options);
            ApplicationBuilder.UseMiddleware<LoggingMiddleware>(options);
        }


        [UsedImplicitly]
        public static void UseErikTheCoderExceptionHandling(this IApplicationBuilder ApplicationBuilder, Action<ExceptionHandlingOptions> ConfigureOptions = null)
        {
            var options = new ExceptionHandlingOptions();
            ConfigureOptions?.Invoke(options);
            ExceptionHandlingMiddleware.Enable(ApplicationBuilder, options);
        }


        [UsedImplicitly]
        public static AuthenticationBuilder AddErikTheCoderAuthentication(this AuthenticationBuilder AuthenticationBuilder, Action<AuthenticationOptions> ConfigureOptions = null)
        {
            return AuthenticationBuilder.AddScheme<AuthenticationOptions, AuthenticationHandler>(AuthenticationHandler.AuthenticationScheme, ConfigureOptions);
        }


        [UsedImplicitly]
        public static void UseErikTheCoderPolicies(this AuthorizationOptions AuthorizationOptions)
        {
            AuthorizationOptions.AddPolicy(Policy.Admin, Policy.VerifyAdmin);
            AuthorizationOptions.AddPolicy(Policy.TheBigLebowski, Policy.VerifyTheBigLebowski);
            AuthorizationOptions.AddPolicy(Policy.Everyone, Policy.VerifyEveryone);
        }
    }
}
