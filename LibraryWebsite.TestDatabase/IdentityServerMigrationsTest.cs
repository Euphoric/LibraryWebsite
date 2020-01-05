using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LibraryWebsite
{
    public class IdentityServerMigrationsTest : IAsyncLifetime
    {
        private static ServiceProvider SetupServices(IConfiguration configuration)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(configuration);
            services.AddTransient<DatabaseMigrations>();
            services.AddLogging();
            services.AddDbContext<LibraryContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));

            services
                .AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<LibraryContext>();

            return services.BuildServiceProvider();
        }

        readonly IConfigurationRoot _configuration;
        readonly ServiceProvider _services;

        public IdentityServerMigrationsTest()
        {
            _configuration =
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.Local.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddInMemoryCollection(new Dictionary<string, string>())
                    .Build();

            _configuration["MigrateOnStartup"] = true.ToString();

            string databaseServer = _configuration.GetConnectionString("TestDatabaseServer");
            string databaseName = "LibraryTestDb_" + Guid.NewGuid();
            string databaseConnectionString = $"{databaseServer}Database={databaseName};";
            _configuration["ConnectionStrings:Database"] = databaseConnectionString;

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