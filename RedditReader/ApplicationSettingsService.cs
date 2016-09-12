using System;
using System.Configuration;

namespace RedditReader
{
    public static class ApplicationSettingsService
    {
        public static string RedditUserName => GetValueFromConfigFile("redditUserName");
        public static string RedditPassword => GetValueFromConfigFile("redditPassword");
        public static string AppId => GetValueFromConfigFile("appId");
        public static string AppSecret => GetValueFromConfigFile("appSecret");

        private static string GetValueFromConfigFile(string key)
        {
            var configValue = ConfigurationManager.AppSettings[key];

            if (String.IsNullOrWhiteSpace(configValue))
            {
                throw new ArgumentException($"Empty config for {key}");
            }
            return configValue;
        }
    }
}
