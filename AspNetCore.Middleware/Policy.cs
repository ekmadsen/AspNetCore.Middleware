using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public static class Policy
    {
        public const string Admin = "Admin";
        public const string Everyone = "Everyone";


        // Include Admin role in all policies.
        public static void VerifyAdmin(AuthorizationPolicyBuilder PolicyBuilder) => PolicyBuilder.RequireClaim(ClaimTypes.Role, Admin);


        public static void VerifyEveryone(AuthorizationPolicyBuilder PolicyBuilder) => PolicyBuilder.RequireAssertion(Context => true);
    }
}
