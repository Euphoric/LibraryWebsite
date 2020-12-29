using System;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryWebsite
{
    public static class EventsIdentityExtension
    {
        public static IIdentityServerBuilder AddApiAuthorization<TUser>(
            this IIdentityServerBuilder builder,
            Action<ApiAuthorizationOptions> configure)
            where TUser : class
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddAspNetIdentity<TUser>()
                .AddIdentityResources()
                .AddApiResources()
                .AddClients()
                .AddSigningCredentials();
            
            //.AddOperationalStore()
            //.ConfigureReplacedServices()

            builder.Services.Configure(configure);

            return builder;
        }
    }
}