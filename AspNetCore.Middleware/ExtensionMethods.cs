using System;
using ErikTheCoder.ServiceContract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public static class ExtensionMethods
    {
        public static Guid GetCorrelationId(this HttpContext HttpContext)
        {
            return HttpContext.Request.Headers.TryGetValue(CustomHttpHeader.CorrelationId, out StringValues correlationId)
                ? Guid.Parse(correlationId[0])
                : Guid.Empty;
        }
    }
}
