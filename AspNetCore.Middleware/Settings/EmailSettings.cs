using JetBrains.Annotations;

namespace ErikTheCoder.AspNetCore.Middleware.Settings
{
    [UsedImplicitly]
    public class EmailSettings : IEmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
