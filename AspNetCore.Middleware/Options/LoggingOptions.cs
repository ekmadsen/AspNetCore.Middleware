using System.Collections.Generic;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware.Options
{
    [UsedImplicitly]
    public class LoggingOptions
    {
        public bool LogRequestParameters { get; [UsedImplicitly] set; }
        public bool LogFbaCookie { get; [UsedImplicitly] set; }
        // ReSharper disable CollectionNeverUpdated.Global
        public List<string> IgnoreUrls { get; }
        public List<string> TruncateUrls { get; }
        // ReSharper restore CollectionNeverUpdated.Global


        public LoggingOptions()
        {
            IgnoreUrls = new List<string>();
            TruncateUrls = new List<string>();
        }
    }
}
