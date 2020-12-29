using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Euphoric.EventModel;
using IdentityServer4;
using LibraryWebsite.Books;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;

namespace LibraryWebsite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddTransient<DatabaseMigrations>();
            services.AddTransient<SampleDataSeeder>();

            services.AddHealthChecks();

            services.AddAuthorization(options =>
            {
                options.AddPolicies(ClaimTypes.Role);
            });

            services
                .AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>();

            services.AddSingleton<UsersRolesMemoryStore>();
            services.AddScoped<IUserStore<ApplicationUser?>, UserEventStore>();
            services.AddScoped<IRoleStore<IdentityRole?>, RoleEventStore>();

            services
                .AddIdentityServer()
                .AddApiAuthorization<ApplicationUser>(options =>
                {
                    options.IdentityResources.AddEmail();
                    options.IdentityResources.Add(new IdentityResource("roles", "User roles",
                        new List<string> {JwtClaimTypes.Role}));

                    options.ApiResources["LibraryWebsiteAPI"].UserClaims = new List<string>
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email,
                        JwtClaimTypes.Role
                    };

                    options.Clients.Add(new IdentityServer4.Models.Client
                    {
                        ClientId = "PublicApi",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                        RequireClientSecret = false
                    });

                    options.Clients.Add(
                        new IdentityServer4.Models.Client()
                        {
                            ClientId = "LibraryWebsite",
                            ClientName = "Library website",
                            AllowedGrantTypes = GrantTypes.Code,

                            // for IClientRequestParametersProvider to work correctly, not used by identity server
                            Properties = {{"Profile", ApplicationProfiles.IdentityServerSPA}},

                            RequireClientSecret = false,
                            RequireConsent = false,

                            RedirectUris = {"/authentication/login-callback"},
                            PostLogoutRedirectUris = {"/authentication/logout-callback"},

                            AllowedScopes =
                            {
                                IdentityServerConstants.StandardScopes.OpenId,
                                IdentityServerConstants.StandardScopes.Profile,
                                IdentityServerConstants.StandardScopes.Email,
                                "roles",
                                "LibraryWebsiteAPI"
                            }
                        }
                    );
                });

            services
                .AddAuthentication()
                .AddIdentityServerJwt();

            services.AddSingleton<IClock>(NodaTime.SystemClock.Instance);

            services.AddSingleton<IEventStore, PersistentEventStore>();
            services.AddSingleton<DomainEventSender>();
            services.AddSingleton<DomainEventFactory>();
            services.AddSingleton(new EventTypeLocator(typeof(BookDomainEvent).Assembly));

            services.AddSingleton<IProjectionContainerFactory, ThreadedProjectionContainerFactory>();

            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionState<BooksListProjection>());
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionListener<BooksListProjection>());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "ASP.NET Core infrastructure code.")]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
