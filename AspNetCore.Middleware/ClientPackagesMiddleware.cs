using ErikTheCoder.AspNetCore.Middleware.Options;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public static class ClientPackagesMiddleware
    {
        [UsedImplicitly]
        public static void Enable(IApplicationBuilder ApplicationBuilder, ClientPackagesOptions Options)
        {
            string requestUrlPath = Options.RequestUrlPath.StartsWith("/")
                ? Options.RequestUrlPath
                : $"/{Options.RequestUrlPath}";
            PhysicalFileProvider fileProvider = new PhysicalFileProvider(Options.FilePath);
            StaticFileOptions staticFileOptions = new StaticFileOptions
            {
                RequestPath = requestUrlPath,
                FileProvider = fileProvider
            };
            ApplicationBuilder.UseStaticFiles(staticFileOptions);
        }
    }
}
