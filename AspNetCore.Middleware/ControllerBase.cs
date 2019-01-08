using System;
using ErikTheCoder.AspNetCore.Middleware.Settings;
using ErikTheCoder.Logging;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public abstract class ControllerBase : Controller
    {
        private bool _retrievedCorrelationId;
        private Guid _correlationId;
        [UsedImplicitly] protected readonly IAppSettings AppSettings;
        [UsedImplicitly] protected readonly ILogger Logger;


        protected ControllerBase(IAppSettings AppSettings, ILogger Logger)
        {
            this.AppSettings = AppSettings;
            this.Logger = Logger;
        }


        protected Guid CorrelationId
        {
            get
            {
                if (_retrievedCorrelationId) return _correlationId;
                _correlationId = HttpContext.GetCorrelationId();
                _retrievedCorrelationId = true;
                return _correlationId;
            }
        }


        protected string GetCallingUsername() => User.Identity.Name ?? "Anonymous";
    }
}
