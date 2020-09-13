using System;
using ErikTheCoder.AspNetCore.Middleware.Settings;
using ErikTheCoder.Logging;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public abstract class ControllerBase : Controller
    {
        private Guid? _correlationId;
        [UsedImplicitly] protected readonly IAppSettings AppSettings;
        [UsedImplicitly] protected readonly ILogger Logger;


        protected ControllerBase(IAppSettings AppSettings, ILogger Logger)
        {
            this.AppSettings = AppSettings;
            this.Logger = Logger;
        }


        [UsedImplicitly]
        protected Guid CorrelationId
        {
            get
            {
                if (_correlationId.HasValue) return _correlationId.Value;
                _correlationId = HttpContext.GetCorrelationId();
                return _correlationId.Value;
            }
        }


        [UsedImplicitly]
        protected string GetCallingUsername() => User.Identity.Name ?? "Anonymous";
    }
}
