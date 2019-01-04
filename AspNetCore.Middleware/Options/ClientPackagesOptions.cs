using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware.Options
{
    [UsedImplicitly]
    public class ClientPackagesOptions
    {
        public string RequestUrlPath { get; [UsedImplicitly] set; }
        public string FilePath { get; [UsedImplicitly] set; }

    }
}
