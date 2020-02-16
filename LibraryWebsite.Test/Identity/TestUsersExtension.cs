using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LibraryWebsite.Identity
{
    public static class TestUsersExtension
    {
        public static async Task LoginAsUser(this HttpClient httpClient, string userName, string password)
        {
            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocument.IsError);

            using var request = new PasswordTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PublicApi",

                UserName = userName,
                Password = password
            };
            var response = await httpClient.RequestPasswordTokenAsync(request);

            Assert.False(response.IsError, response.Error);
            Assert.NotNull(response.AccessToken);

            httpClient.SetBearerToken(response.AccessToken);
        }

        public static async Task AddTestingUsers(this IServiceProvider services)
        {
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