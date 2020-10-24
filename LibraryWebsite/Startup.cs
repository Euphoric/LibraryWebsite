using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

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

            services.AddDbContext<LibraryContext>(options=>options.UseSqlServer(Configuration.GetConnectionString("Database")));
            services.AddTransient<DatabaseMigrations>();
            services.AddTransient<SampleDataSeeder>();

            services.AddHealthChecks()
                .AddDbContextCheck<LibraryContext>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.IsAdmin, policy => { policy.RequireClaim(ClaimTypes.Role, Role.Admin); });
                options.AddPolicy(Policies.IsLibrarian, policy => { policy.RequireClaim(ClaimTypes.Role, Role.Librarian); });
                options.AddPolicy(Policies.CanEditBooks, policy => policy.RequireClaim(ClaimTypes.Role,Role.Librarian) );
            });

            services
                .AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<LibraryContext>();

            services
                .AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, LibraryContext>(options =>
                {
                    options.IdentityResources.AddEmail();

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

                    options.Clients.AddIdentityServerSPA("LibraryWebsite", config =>
                    {
                        config.WithLogoutRedirectUri("/authentication/logout-callback");
                        config.WithRedirectUri("/authentication/login-callback");
                    });
                });
            
            services
                .AddAuthentication()
                .AddIdentityServerJwt();
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
