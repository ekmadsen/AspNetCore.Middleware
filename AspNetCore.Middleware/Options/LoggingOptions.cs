using System.Collections.Generic;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware.Options
{
    [UsedImplicitly]
    public class LoggingOptions
    {
        public bool LogRequestParameters { get; [UsedImplicitly] set; }
        public List<string> IgnoreUrls { get; }
        public List<string> TruncateUrls { get; }


        public LoggingOptions()
        {
            IgnoreUrls = new List<string>();
            TruncateUrls = new List<string>();
        }
    }
}
