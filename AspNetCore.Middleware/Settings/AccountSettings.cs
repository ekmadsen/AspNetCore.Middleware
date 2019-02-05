using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware.Settings
{
    [UsedImplicitly]
    public class AccountSettings : IAccountSettings
    {
        public string ConfirmationUrl { get; set; }
        public string ResetUrl { get; set; }
    }
}
