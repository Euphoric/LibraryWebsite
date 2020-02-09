using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace LibraryWebsite
{
    public static class TestServerCreator
    {
        public static TestServer CreateTestServer(ITestOutputHelper outputHelper)
        {
            var builder = WebHost.CreateDefaultBuilder()
                .UseEnvironment("Testing")
                .UseStartup<Startup>()
                .ConfigureTestServices(TestingStartup.ConfigureServices)
                .ConfigureLogging(logging => logging.AddXUnit(outputHelper));

            var testServer = new TestServer(builder);
            testServer.BaseAddress = new Uri("https://localhost/"); // use HTTPS for all requests
            return testServer;
        }

        public static async Task AddTestingUsers(TestServer testServer)
        {
            testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));

            var services = testServer.Host.Services;
            using (var roleStore = services.GetRequiredService<RoleManager<IdentityRole>>())
            {
                await roleStore.CreateAsync(new IdentityRole(Role.Admin));
                await roleStore.CreateAsync(new IdentityRole(Role.Librarian));
                await roleStore.CreateAsync(new IdentityRole(Role.Reader));
            }

            using var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var adminUser = new ApplicationUser
            {
                UserName = "Admin",
                Email = "Admin@library.com",
                NormalizedUserName = "Admin name"
            };
            await userManager.CreateAsync(adminUser, "Administrator_1");
            await userManager.AddToRolesAsync(adminUser, new[] {Role.Admin, Role.Librarian});

            var ordinaryUser = new ApplicationUser
            {
                UserName = "User",
                Email = "User@library.com",
                NormalizedUserName = "Admin name"
            };
            await userManager.CreateAsync(ordinaryUser, "User_1");
            await userManager.AddToRolesAsync(ordinaryUser, new[] {Role.Librarian});
        }
    }
}