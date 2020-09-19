using ErikTheCoder.Logging;
using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CorrelationIdAccessor(IHttpContextAccessor HttpContextAccessor)
        {
            _httpContextAccessor = HttpContextAccessor;
        }


        public Guid GetCorrelationId() => _httpContextAccessor.HttpContext.GetCorrelationId();
    }
}
