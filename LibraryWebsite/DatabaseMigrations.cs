using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;

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

        public async ValueTask EnsureDatabaseSchemaIsCurrent()
        {
            var shouldMigrate = _configuration.GetValue("MigrateOnStartup", false);
            if (!shouldMigrate)
                return;

            _logger.LogInformation("Migrating to newest database schema.");

            await Policy
                .Handle<SqlException>()
                .RetryAsync(3, OnMigrationRetry)
                .ExecuteAsync(MigrateAndSeedDatabase);
        }

        private void OnMigrationRetry(Exception failedException, int retryCount)
        {
            _logger.LogWarning(failedException, "Failed try {0} of migrating database.", retryCount);
        }

        private async Task MigrateAndSeedDatabase()
        {
            using var scope = _services.CreateScope();

            var libraryContext = scope.ServiceProvider.GetRequiredService<LibraryContext>();

            await MigrateDatabase(libraryContext);
            await SeedSampleData(scope.ServiceProvider.GetRequiredService<SampleDataSeeder>());
        }

        private static async ValueTask MigrateDatabase(LibraryContext libraryContext)
        {
            await libraryContext.Database.MigrateAsync();
        }

        private async ValueTask SeedSampleData(SampleDataSeeder sampleDataSeeder)
        {
            if (_configuration.GetValue("SeedSampleData", false))
            {
                await sampleDataSeeder.SetupExampleData();
            }
        }

        public async ValueTask<bool> DoesDatabaseExists()
        {
            var database = _services.GetService<LibraryContext>().Database;
            return await database.CanConnectAsync();
        }

        public async ValueTask<bool> HasNewestMigrations()
        {
            var database = _services.GetService<LibraryContext>().Database;
            var pendingMigrations = await database.GetPendingMigrationsAsync();
            return !pendingMigrations.Any();
        }
    }
}
