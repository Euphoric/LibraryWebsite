using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

            builder.Services.AddHttpClient("api", options => options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler(sp =>
                {
                    // adds message handler that adds authorization tokens to outgoing requests
                    var handler = new CustomAuthorizationMessageHandler(sp.GetRequiredService<IAccessTokenProvider>())
                        .ConfigureHandler(
                            authorizedUrls: new[] { builder.HostEnvironment.BaseAddress },
                            scopes: new[] { "LibraryWebsiteAPI" });

                    return handler;
                });

            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("api"));

            // adds identity server configuration retrieved from configuration endpoint
            builder.Services.AddApiAuthorization(options =>
            {
                options.ProviderOptions.ConfigurationEndpoint = "_configuration/LibraryWebsite";
                // make Blazor authorization recognize role claims
                options.UserOptions.RoleClaim = "role";
            });

            // setup common policies
            builder.Services.AddAuthorizationCore(options => { options.AddPolicies("role"); });

            await builder.Build().RunAsync();
        }
    }
}
