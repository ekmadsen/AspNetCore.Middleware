using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;


namespace ErikTheCoder.AspNetCore.Middleware.Options
{
    public class AuthenticationOptions : AuthenticationSchemeOptions
    {
        [UsedImplicitly]
        public AuthenticationIdentities Identities { get; [UsedImplicitly] set; }


        public AuthenticationOptions()
        {
            Identities = new AuthenticationIdentities();
        }
    }
}
