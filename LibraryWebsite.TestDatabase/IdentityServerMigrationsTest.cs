using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LibraryWebsite
{
    public class IdentityServerMigrationsTest : IAsyncLifetime
    {
        readonly ServiceProvider _services;

        public IdentityServerMigrationsTest()
        {
            _services = DatabaseTestServices.SetupDatabaseTestServices();
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
        public async Task Identity_schema_was_migrated()
        {
            var dbMigrations = _services.GetRequiredService<DatabaseMigrations>();
            await dbMigrations.EnsureDatabaseSchemaIsCurrent();

            using var roleManager = _services.GetRequiredService<RoleManager<IdentityRole>>();
            using var userManager = _services.GetRequiredService<UserManager<ApplicationUser>>();

            await roleManager.CreateAsync(new IdentityRole("TestRole"));

            var user = new ApplicationUser() { UserName = "TestUser" };
            await userManager.CreateAsync(user, "TestPassword_1");
            await userManager.AddToRoleAsync(user, "TestRole");

            var storedUsers = await userManager.Users.Where(x => x.UserName == "TestUser").ToListAsync();

            var storedUser = Assert.Single(storedUsers);

            var passwordIsValid = await userManager.CheckPasswordAsync(storedUser, "TestPassword_1");
            Assert.True(passwordIsValid);
        }
    }
}