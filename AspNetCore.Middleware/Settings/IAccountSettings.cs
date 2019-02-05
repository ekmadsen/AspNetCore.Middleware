using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware.Settings
{
    public interface IAccountSettings
    {
        [UsedImplicitly] string ConfirmationUrl { get; set; }
        [UsedImplicitly] string ResetUrl { get; set; }
    }
}
