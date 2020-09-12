using System;
using ErikTheCoder.AspNetCore.Middleware.Options;
using ErikTheCoder.Logging;
using ErikTheCoder.Utilities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public static class ClientPackagesMiddleware
    {
        [UsedImplicitly]
        public static void Enable(IApplicationBuilder ApplicationBuilder, ClientPackagesOptions Options)
        {
            try
            {
                var requestUrlPath = Options.RequestUrlPath.StartsWith("/")
                    ? Options.RequestUrlPath
                    : $"/{Options.RequestUrlPath}";
                var fileProvider = new PhysicalFileProvider(Options.FilePath);
                var staticFileOptions = new StaticFileOptions
                {
                    RequestPath = requestUrlPath,
                    FileProvider = fileProvider
                };
                ApplicationBuilder.UseStaticFiles(staticFileOptions);
            }
            catch (Exception exception)
            {
                var logger = ApplicationBuilder.ApplicationServices.GetService<ILogger>();
                logger?.Log($"Failed to enable {nameof(ClientPackagesMiddleware)}.  {exception.GetSummary(true, true)}");
                throw;
            }
        }
    }
}
