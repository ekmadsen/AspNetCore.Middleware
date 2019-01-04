using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public static class CorrelationId
    {
        public static Guid Get(HttpContext HttpContext)
        {
            return HttpContext.Request.Headers.TryGetValue(CustomHttpHeader.CorrelationId, out StringValues correlationId)
                ? Guid.Parse(correlationId[0])
                : Guid.Empty;
        }
    }
}
