using ErikTheCoder.Logging.Settings;
using ErikTheCoder.ServiceProxy.Settings;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware.Settings
{
    public interface IAppSettings
    {
        [UsedImplicitly] DatabaseLoggerSettings Logger { get; [UsedImplicitly] set; }
        [UsedImplicitly] EmailSettings Email { get; [UsedImplicitly] set; }
        [UsedImplicitly] string Database { get; [UsedImplicitly] set; }
        [UsedImplicitly] ServiceProxySettings ServiceProxies { get; [UsedImplicitly] set; }
        [UsedImplicitly] AuthenticationIdentities AuthenticationIdentities { get; set; }
        [UsedImplicitly] string CredentialSecret { get; [UsedImplicitly] set; }
        [UsedImplicitly] int AdminCredentialExpirationMinutes { get; [UsedImplicitly] set; }
        [UsedImplicitly] int NonAdminCredentialExpirationMinutes { get; [UsedImplicitly] set; }
        [UsedImplicitly] bool EnableHttpContextFilter { get; [UsedImplicitly] set; }
        [UsedImplicitly] int ShortTermCacheExpirationMinutes { get; [UsedImplicitly] set; }
        [UsedImplicitly] int LongTermCacheExpirationMinutes { get; [UsedImplicitly] set; }
    }
}
