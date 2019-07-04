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
        protected TInterface ParseConfigurationFile<TInterface, TClass>() where TClass : TInterface
        {
            const string environmentalVariableName = "ASPNETCORE_ENVIRONMENT";
            string environment = Environment.GetEnvironmentVariable(environmentalVariableName) ?? Microsoft.AspNetCore.Hosting.EnvironmentName.Development;
            string directory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) ?? string.Empty;
            string configurationFile = Path.Combine(directory, "appSettings.json");
            if (!File.Exists(configurationFile)) throw new Exception($"Configuration file not found at {configurationFile}.");
            JObject configuration = JObject.Parse(File.ReadAllText(configurationFile));
            return configuration.GetValue(environment).ToObject<TClass>();
        }
    }
}
