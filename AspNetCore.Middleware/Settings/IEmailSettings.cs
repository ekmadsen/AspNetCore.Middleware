using JetBrains.Annotations;

namespace ErikTheCoder.AspNetCore.Middleware.Settings
{
    public interface IEmailSettings
    {
        [UsedImplicitly] string Host { get; set; }
        [UsedImplicitly] int Port { get; set; }
        [UsedImplicitly] bool EnableSsl { get; set; }
        [UsedImplicitly] string Username { get; set; }
        [UsedImplicitly] string Password { get; set; }
        [UsedImplicitly] string From { get; set; }
        [UsedImplicitly] string ConfirmationUrl { get; set; }
    }
}
