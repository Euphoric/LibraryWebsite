using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebsite
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            IWebHost webHost = CreateWebHostBuilder(args).Build();

            EnsureDatabaseSchemaIsCurrent(webHost.Services);

            webHost.Run();
        }

        private static void EnsureDatabaseSchemaIsCurrent(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();
            var shouldMigrate = config.GetValue("MigrateOnStartup", false);
            if (!shouldMigrate)
                return;

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigration");
            logger.LogInformation("Migrating to newest database schema.");

            using (var scope = services.CreateScope())
            {
                scope.ServiceProvider.GetService<LibraryContext>().Database.Migrate();

                if (scope.ServiceProvider.GetService<IWebHostEnvironment>().IsDevelopment())
                    scope.ServiceProvider.GetService<LibraryContext>().SetupExampleData();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
