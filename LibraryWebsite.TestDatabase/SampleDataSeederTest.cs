using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euphoric.EventModel;
using LibraryWebsite.Books;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LibraryWebsite
{
    public class SampleDataSeederTest : IAsyncLifetime
    {
        readonly IConfiguration _configuration;
        readonly ServiceProvider _services;

        public SampleDataSeederTest()
        {
            _services = DatabaseTestServices.SetupDatabaseTestServices();
            _configuration = _services.GetRequiredService<IConfiguration>();
        }

        public async Task InitializeAsync()
        {
            await _services.GetRequiredService<DatabaseMigrations>().EnsureDatabaseSchemaIsCurrent();
        }

        public async Task DisposeAsync()
        {
            await _services.GetRequiredService<LibraryContext>().Database.EnsureDeletedAsync();
        }
        
        [Fact]
        public async Task Doesnt_seed_sample_data_after_migration_if_not_configured()
        {
            _configuration["SeedSampleData"] = "false";

            var seeder = _services.GetRequiredService<SampleDataSeeder>();

            await seeder.SetupExampleData();

            // books were not seeded
            Assert.False(_services.GetRequiredService<IProjectionState<BooksListProjection>>().State.ListBooks().Any());
            
            // users were not seeded
            var userManager = _services.GetRequiredService<UserManager<ApplicationUser>>();
            Assert.Empty(await userManager.GetUsersInRoleAsync(Role.Admin));
        }

        [Fact]
        public async Task Seeds_sample_data_after_migration()
        {
            _configuration["SeedSampleData"] = "true";

            var seeder = _services.GetRequiredService<SampleDataSeeder>();

            await seeder.SetupExampleData();

            // books were seeded
            Assert.True(_services.GetRequiredService<IProjectionState<BooksListProjection>>().State.ListBooks().Any());

            // users were seeded
            var userManager = _services.GetRequiredService<UserManager<ApplicationUser>>();
            
            Assert.NotEmpty(await userManager.GetUsersInRoleAsync(Role.Admin));
            Assert.NotEmpty(await userManager.GetUsersInRoleAsync(Role.Librarian));
            Assert.NotEmpty(await userManager.GetUsersInRoleAsync(Role.Reader));
        }
    }
}
