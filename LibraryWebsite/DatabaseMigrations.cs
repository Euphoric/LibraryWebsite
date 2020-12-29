using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.SqlClient;
using Polly;

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
                .WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(retryCount * 5), OnMigrationRetry)
                .ExecuteAsync(MigrateAndSeedDatabase);
        }

        private void OnMigrationRetry(Exception exception, TimeSpan nextTryTimeout, int retryCount, Context context)
        {
            _logger.LogWarning(exception, "Failed try {0} of migrating database. Waiting {1} before next try.", retryCount, nextTryTimeout);
        }

        private async Task MigrateAndSeedDatabase()
        {
            using var scope = _services.CreateScope();

            var libraryContext = scope.ServiceProvider.GetRequiredService<LibraryContext>();

            await MigrateDatabase(libraryContext);
        }

        private static async ValueTask MigrateDatabase(LibraryContext libraryContext)
        {
            await libraryContext.Database.MigrateAsync();
        }

        public async ValueTask<bool> DoesDatabaseExists()
        {
            var database = _services.GetRequiredService<LibraryContext>().Database;
            return await database.CanConnectAsync();
        }

        public async ValueTask<bool> HasNewestMigrations()
        {
            var database = _services.GetRequiredService<LibraryContext>().Database;
            var pendingMigrations = await database.GetPendingMigrationsAsync();
            return !pendingMigrations.Any();
        }
    }
}
