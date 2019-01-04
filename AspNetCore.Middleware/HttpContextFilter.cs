using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public class HttpContextFilter : IHttpContextFilter
    {
        public Guid CorrelationId { get; set; }
        public string ClientIpAddress { get; set; }
        public string ServerIpAddress { get; set; }
        public User User { get; set; }
        public TimeSpan ActionExecutionDuration { get; set; }
        public TimeSpan ResultExecutionDuration { get; set; }


        public async Task OnActionExecutionAsync(ActionExecutingContext Context, ActionExecutionDelegate Next)
        {
            CorrelationId = Context.HttpContext.GetCorrelationId();
            ClientIpAddress = $"{Context.HttpContext.Connection.RemoteIpAddress}:{Context.HttpContext.Connection.RemotePort}";
            ServerIpAddress = $"{Context.HttpContext.Connection.LocalIpAddress}:{Context.HttpContext.Connection.RemotePort}";
            User = User.ParseClaims(Context.HttpContext.User.Claims);
            Stopwatch stopwatch = Stopwatch.StartNew();
            await Next();
            stopwatch.Stop();
            ActionExecutionDuration = stopwatch.Elapsed;
        }


        public async Task OnResultExecutionAsync(ResultExecutingContext Context, ResultExecutionDelegate Next)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await Next();
            stopwatch.Stop();
            ResultExecutionDuration = stopwatch.Elapsed;
        }
    }
}
