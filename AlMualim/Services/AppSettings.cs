using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AlMualim.Services
{
    public static class AppSettings
    {
        public static readonly IConfigurationRoot Config;

        static AppSettings()
        {
            // Build configuration service
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Config = builder.Build();
        }

        public static string Get(string key) => Config.GetValue<string>(key);

    }
}