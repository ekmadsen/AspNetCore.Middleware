using System;
using ErikTheCoder.Logging;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;


namespace ErikTheCoder.AspNetCore.Middleware.Options
{
    [UsedImplicitly]
    public class ExceptionHandlingOptions
    {
        public string AppName { get; [UsedImplicitly] set; }
        public string ProcessName { get; [UsedImplicitly] set; }
        public ExceptionResponseFormat? ExceptionResponseFormat { get; [UsedImplicitly] set; }
        public bool IncludeDetails { get; [UsedImplicitly] set; }
        public Action<HttpContext, SimpleException> ResponseHandler { get; [UsedImplicitly] set; }
    }
}
