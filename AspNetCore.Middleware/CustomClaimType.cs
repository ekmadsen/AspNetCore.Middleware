namespace ErikTheCoder.AspNetCore.Middleware
{
    public static class CustomClaimType
    {
        private const string _prefix = "https://schema.erikthecoder.net/claims/";
        // Cannot use string interpolation for constants.
        public const string Nickname = _prefix + "nickname";
        public const string Ability = _prefix + "ability";
    }
}
