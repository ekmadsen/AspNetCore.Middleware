using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public interface IHttpContextFilter : IAsyncActionFilter, IAsyncResultFilter
    {
        // ReSharper disable UnusedMemberInSuper.Global
        // ReSharper disable UnusedMember.Global
        Guid CorrelationId { get; set; }
        string ClientIpAddress { get; set; }
        string ServerIpAddress { get; set; }
        User User { get; [UsedImplicitly] set; }
        TimeSpan ActionExecutionDuration { get; set; }
        TimeSpan ResultExecutionDuration { get; set; }
        // ReSharper restore UnusedMember.Global
        // ReSharper restore UnusedMemberInSuper.Global
    }
}
