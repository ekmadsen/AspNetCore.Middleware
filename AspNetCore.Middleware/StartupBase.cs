using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public abstract class StartupBase
    {
        [UsedImplicitly]
        protected TInterface ParseConfigurationFile<TInterface, TClass>() where TClass : class, TInterface
        {
            const string environmentalVariableName = "ASPNETCORE_ENVIRONMENT";
            var environment = Environment.GetEnvironmentVariable(environmentalVariableName) ?? Microsoft.AspNetCore.Hosting.EnvironmentName.Development;
            var directory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) ?? string.Empty;
            var configurationFile = Path.Combine(directory, "appSettings.json");
            if (!File.Exists(configurationFile)) throw new Exception($"Configuration file not found at {configurationFile}.");
            var configuration = JObject.Parse(File.ReadAllText(configurationFile));
            return configuration.GetValue(environment)?.ToObject<TClass>();
        }
    }
}
