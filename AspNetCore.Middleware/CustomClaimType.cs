using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public static class CustomClaimType
    {
        private const string _namespace = "https://schema.erikthecoder.net/claims/";

        // Can't use string interpolation to assign a constant.
        [UsedImplicitly] public const string FirstName = _namespace + "firstname";
        [UsedImplicitly] public const string LastName = _namespace + "lastname";
        [UsedImplicitly] public const string SecurityToken = _namespace + "securitytoken";
    }
}
