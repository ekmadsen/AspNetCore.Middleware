using ErikTheCoder.ServiceContract;
using Microsoft.AspNetCore.Authorization;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public static class Policy
    {
        public const string Admin = "Admin";
        public const string TheBigLebowski = "The Big Lebowski";
        public const string Everyone = "Everyone";


        public static void VerifyAdmin(AuthorizationPolicyBuilder PolicyBuilder)
        {
            PolicyBuilder.RequireAssertion(Context =>
            {
                var user = User.ParseClaims(Context.User.Claims);
                return user.Roles.Contains(Admin);
            });
        }


        public static void VerifyTheBigLebowski(AuthorizationPolicyBuilder PolicyBuilder)
        {
            PolicyBuilder.RequireAssertion(Context =>
            {
                var user = User.ParseClaims(Context.User.Claims);
                if (user.Claims.TryGetValue(CustomClaimType.Nickname, out var nicknames))
                {
                    if (nicknames.Contains("The Dude")) 
                    {
                        if (user.Claims.TryGetValue(CustomClaimType.Ability, out var abilities)) return abilities.Contains("Make White Russian") && abilities.Contains("Abide");
                    }
                }
                return false;
            });
        }


        public static void VerifyEveryone(AuthorizationPolicyBuilder PolicyBuilder) => PolicyBuilder.RequireAssertion(Context => true);
    }
}
