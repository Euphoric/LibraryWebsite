using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LibraryWebsite
{
    public class Database_migrations_test : IAsyncLifetime
    {
        private static ServiceProvider SetupServices(IConfiguration configuration)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(configuration);
            services.AddTransient<DatabaseMigrations>();
            services.AddLogging();
            services.AddDbContext<LibraryContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));

            return services.BuildServiceProvider();
        }

        readonly IConfigurationRoot _configuration;
        readonly ServiceProvider _services;

        public Database_migrations_test()
        {
            string databaseName = "LibraryTestDb_" + Guid.NewGuid();

            string databaseServer = 
                Environment.GetEnvironmentVariable("DATABASE_SERVER") 
                ?? "Server=localhost\\SQLEXPRESS;Trusted_Connection=True;";

            string databaseConnectionString = $"{databaseServer}Database={databaseName};";

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>() {
                    { "ConnectionStrings:Database", databaseConnectionString }
                })
                ;

            _configuration = configurationBuilder.Build();

            _configuration["MigrateOnStartup"] = true.ToString();

            _services = SetupServices(_configuration);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _services.GetService<LibraryContext>().Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task Database_doesnt_exist_at_first()
        {
            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();
            Assert.False(await dbMigrations.DoesDatabaseExists());
        }

        [Fact]
        public async Task Nothing_happens_if_migrations_are_configured_off()
        {
            _configuration["MigrateOnStartup"] = false.ToString();

            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            Assert.False(await dbMigrations.DoesDatabaseExists());

            dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.False(await dbMigrations.DoesDatabaseExists());
        }

        [Fact]
        public async Task Database_is_created_if_it_doesnt_exist()
        {
            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            Assert.False(await dbMigrations.DoesDatabaseExists());

            dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.True(await dbMigrations.DoesDatabaseExists());
        }

        [Fact]
        public async Task Database_schema_is_newest_after_migration()
        {
            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.True(await dbMigrations.HasNewestMigrations());
        }

        [Fact]
        public async Task Doesnt_seed_sample_data_after_migration_if_not_configured()
        {
            _configuration["SeedSampleData"] = false.ToString();

            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.False(await _services.GetRequiredService<LibraryContext>().Books.AnyAsync());
        }

        [Fact]
        public async Task Seeds_sample_data_after_migration()
        {
            _configuration["SeedSampleData"] = true.ToString();

            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.True(await _services.GetRequiredService<LibraryContext>().Books.AnyAsync());
        }
    }
}
