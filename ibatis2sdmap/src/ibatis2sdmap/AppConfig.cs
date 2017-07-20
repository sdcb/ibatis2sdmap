using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ibatis2sdmap
{
    public static class AppConfig
    {
        public static IConfigurationRoot Configuration { get; set; }

        static AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appconfig.json");
            Configuration = builder.Build();
        }

        public static string IBatisXmlDirectory => Environment.GetCommandLineArgs()[1];

        public static string DestinationDirectory => Configuration[nameof(DestinationDirectory)];

        public const string NsPrefix = "http://ibatis.apache.org/mapping";
    }
}
