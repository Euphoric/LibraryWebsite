using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euphoric.EventModel;
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
        public static async Task<int> Main(string[] args)
        {
            IWebHost webHost = CreateWebHostBuilder(args).Build();

            try
            {
                if (webHost.Services.GetRequiredService<IEventStore>() is PersistentEventStore pes)
                {
                    await pes.SubscribeClient();
                }
                
                await webHost.Services
                    .GetRequiredService<DatabaseMigrations>()
                    .EnsureDatabaseSchemaIsCurrent();

                await webHost.Services
                    .GetRequiredService<SampleDataSeeder>()
                    .SetupExampleData();
                
                await webHost.RunAsync();

                return 0;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                var logger = webHost.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
                logger.LogCritical(e, "Error when starting or running web server.");

                return 1;
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    if (builderContext.HostingEnvironment.IsDevelopment())
                    {
                        config.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
                    }
                });
    }
}
