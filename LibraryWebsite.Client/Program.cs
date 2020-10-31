using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryWebsite
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("api", options=>options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler(sp => 
                {
                    var handler = new CustomAuthorizationMessageHandler(sp.GetRequiredService<IAccessTokenProvider>())
                        .ConfigureHandler(
                            authorizedUrls: new[] { builder.HostEnvironment.BaseAddress },
                            scopes: new[] { "LibraryWebsiteAPI" });

                    return handler;
                });

            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("api"));

            builder.Services.AddOidcAuthentication(options =>
            {
                // TODO : load from _configuration/LibraryWebsite endpoint
                options.ProviderOptions.Authority = builder.HostEnvironment.BaseAddress;
                options.ProviderOptions.ClientId = "LibraryWebsite";
                options.ProviderOptions.DefaultScopes.Add("openid");
                options.ProviderOptions.DefaultScopes.Add("profile");
                options.ProviderOptions.DefaultScopes.Add("email");
                options.ProviderOptions.DefaultScopes.Add("roles");
                options.ProviderOptions.DefaultScopes.Add("LibraryWebsiteAPI");
                
                options.ProviderOptions.ResponseType = "code";
                options.UserOptions.RoleClaim = ClaimTypes.Role;
            });

            builder.Services.AddAuthorizationCore(options => { options.AddPolicies(); });

            await builder.Build().RunAsync();
        }
    }
}
