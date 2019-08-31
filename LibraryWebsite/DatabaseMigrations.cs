using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebsite
{
    public class DatabaseMigrations
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DatabaseMigrations> _logger;
        private readonly IConfiguration _configuration;

        public DatabaseMigrations(
            IServiceProvider services, 
            ILogger<DatabaseMigrations> logger,
            IConfiguration configuration)
        {
            _services = services;
            _logger = logger;
            _configuration = configuration;
        }

        public void EnsureDatabaseSchemaIsCurrent()
        {
            var shouldMigrate = _configuration.GetValue("MigrateOnStartup", false);
            if (!shouldMigrate)
                return;

            _logger.LogInformation("Migrating to newest database schema.");

            using (var scope = _services.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                MigrateDatabase(serviceProvider);
            }
        }

        private static void MigrateDatabase(IServiceProvider serviceProvider)
        {
            serviceProvider.GetService<LibraryContext>().Database.Migrate();

            if (serviceProvider.GetService<IWebHostEnvironment>().IsDevelopment())
                serviceProvider.GetService<LibraryContext>().SetupExampleData();
        }
    }
}
