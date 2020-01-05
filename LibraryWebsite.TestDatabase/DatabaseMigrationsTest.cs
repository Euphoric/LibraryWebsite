using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LibraryWebsite
{
    public class DatabaseMigrationsTest : IAsyncLifetime
    {
        readonly IConfiguration _configuration;
        readonly ServiceProvider _services;

        public DatabaseMigrationsTest()
        {
            _services = DatabaseTestServices.SetupDatabaseTestServices();
            _configuration = _services.GetRequiredService<IConfiguration>();
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

            await dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.False(await dbMigrations.DoesDatabaseExists());
        }

        [Fact]
        public async Task Database_is_created_if_it_doesnt_exist()
        {
            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            Assert.False(await dbMigrations.DoesDatabaseExists());

            await dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.True(await dbMigrations.DoesDatabaseExists());
        }

        [Fact]
        public async Task Database_schema_is_newest_after_migration()
        {
            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            await dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.True(await dbMigrations.HasNewestMigrations());
        }

        [Fact]
        public async Task Doesnt_seed_sample_data_after_migration_if_not_configured()
        {
            _configuration["SeedSampleData"] = false.ToString();

            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            await dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.False(await _services.GetRequiredService<LibraryContext>().Books.AnyAsync());
        }

        [Fact]
        public async Task Seeds_sample_data_after_migration()
        {
            _configuration["SeedSampleData"] = true.ToString();

            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();

            await dbMigrations.EnsureDatabaseSchemaIsCurrent();

            Assert.True(await _services.GetRequiredService<LibraryContext>().Books.AnyAsync());
        }
    }
}
