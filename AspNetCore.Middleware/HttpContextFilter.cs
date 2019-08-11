using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ErikTheCoder.Logging;
using ErikTheCoder.ServiceContract;
using ErikTheCoder.Utilities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


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
            ILogger logger = Context.HttpContext.RequestServices.GetService<ILogger>();
            try
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
            catch (Exception exception)
            {
                logger?.Log(Context.HttpContext.GetCorrelationId(), $"Exception occurred in {nameof(HttpContextFilter)}.  {exception.GetSummary(true, true)}");
                throw;
            }
        }


        public async Task OnResultExecutionAsync(ResultExecutingContext Context, ResultExecutionDelegate Next)
        {
            ILogger logger = Context.HttpContext.RequestServices.GetService<ILogger>();
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                await Next();
                stopwatch.Stop();
                ResultExecutionDuration = stopwatch.Elapsed;
            }
            catch (Exception exception)
            {
                logger?.Log(Context.HttpContext.GetCorrelationId(), $"Exception occurred in {nameof(HttpContextFilter)}.  {exception.GetSummary(true, true)}");
                throw;
            }
        }
    }
}
