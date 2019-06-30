using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ErikTheCoder.Logging;
using ErikTheCoder.Utilities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using AuthenticationOptions = ErikTheCoder.AspNetCore.Middleware.Options.AuthenticationOptions;
using ILogger = ErikTheCoder.Logging.ILogger;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
    {
        public const string AuthenticationScheme = "ErikTheCoder Token";
        [UsedImplicitly] public const string HttpHeaderName = "Authorization";
        [UsedImplicitly] public const string TokenPrefix = "ErikTheCoder ";
        private readonly ILogger _logger;


        public AuthenticationHandler(IOptionsMonitor<AuthenticationOptions> Options, ILoggerFactory LoggerFactory, UrlEncoder Encoder, ISystemClock Clock, ILogger Logger)
            : base(Options, LoggerFactory, Encoder, Clock)
        {
            _logger = Logger;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                return await Authenticate();
            }
            catch (Exception exception)
            {
                _logger?.Log(Context.GetCorrelationId(), $"Exception occurred in {nameof(AuthenticationHandler)}.  {exception.GetSummary(true, true)}");
                throw;
            }
        }


        private async Task<AuthenticateResult> Authenticate()
        {
            if (!Request.Headers.TryGetValue(HttpHeaderName, out StringValues authorizationValues)) return await Task.FromResult(AuthenticateResult.Fail($"{HttpHeaderName} header not found.")); // Indicate failure.
            string token = authorizationValues.ToString();
            // TODO: Replace foreach-if with a key lookup.
            foreach (AuthenticationIdentity authenticationIdentity in Options.Identities)
            {
                if (token == $"{TokenPrefix}{authenticationIdentity.Token}")
                {
                    // Authorization token is valid.
                    // Create claims identity, add roles, and add claims.
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(AuthenticationScheme);
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, authenticationIdentity.Username));
                    foreach (string role in authenticationIdentity.Roles) claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                    foreach ((string claimType, HashSet<string> claimValues) in authenticationIdentity.Claims) foreach (string claimValue in claimValues) claimsIdentity.AddClaim(new Claim(claimType, claimValue));
                    // Create authentication ticket and indicate success.
                    AuthenticationTicket authenticationTicket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
                    return await Task.FromResult(AuthenticateResult.Success(authenticationTicket));
                }
            }
            // Token does not match any known authentication identities.
            // Indicate failure.
            return await Task.FromResult(AuthenticateResult.Fail($"Invalid {HttpHeaderName} header."));
        }
    }
}
