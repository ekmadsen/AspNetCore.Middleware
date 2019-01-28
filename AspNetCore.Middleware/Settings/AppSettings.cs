using ErikTheCoder.Logging.Settings;
using ErikTheCoder.ServiceProxy.Settings;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware.Settings
{
    [UsedImplicitly]
    public class AppSettings : IAppSettings
    {
        public DatabaseLoggerSettings Logger { get; set; }
        public EmailSettings Email { get; set; }
        public string Database { get; set; }
        public ServiceProxySettings ServiceProxies { get; set; }
        public AuthenticationIdentities AuthenticationIdentities { get; set; }
        public string CredentialSecret { get; set; }
        public int AdminCredentialExpirationMinutes { get; set; }
        public int NonAdminCredentialExpirationMinutes { get; set; }
        public bool EnableHttpContextFilter { get; set; }
        public int ShortTermCacheExpirationMinutes { get; set; }
        public int LongTermCacheExpirationMinutes { get; set; } 
    }
}
