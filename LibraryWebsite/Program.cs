using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryWebsite
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            IWebHost webHost = CreateWebHostBuilder(args).Build();

            try
            {
                webHost.Services
                    .GetRequiredService<DatabaseMigrations>()
                    .EnsureDatabaseSchemaIsCurrent();

                webHost.Run();

                return 0;
            }
            catch (Exception e)
            {
                var logger = webHost.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
                logger.LogCritical(e, "Error when starting or running web server.");

                return 1;
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
                });
    }
}
