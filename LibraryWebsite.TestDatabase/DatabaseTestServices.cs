using System;
using System.Collections.Generic;
using Euphoric.EventModel;
using LibraryWebsite.Books;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;

namespace LibraryWebsite
{
    public static class DatabaseTestServices
    {
        private static ServiceProvider SetupServices(IConfiguration configuration, Action<ServiceCollection> configure)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(configuration);
            services.AddTransient<DatabaseMigrations>();
            services.AddLogging();
            services.AddDbContext<LibraryContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));
            services.AddTransient<SampleDataSeeder>();

            services
                .AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<LibraryContext>();

            services.AddSingleton<IClock>(new FakeClock(Instant.FromUtc(2020, 2, 3, 4, 5)));

            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<DomainEventSender>();
            services.AddSingleton<DomainEventFactory>();
            services.AddSingleton(new EventTypeLocator(typeof(BookDomainEvent).Assembly));

            services.AddSingleton<IProjectionContainerFactory, SynchronousProjectionContainerFactory>();

            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionState<BooksListProjection>());
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionListener<BooksListProjection>());

            configure(services);

            return services.BuildServiceProvider();
        }

        public static ServiceProvider SetupDatabaseTestServices(Action<ServiceCollection>? configure = null)
        {
            configure ??= (a => { });

            var configuration =
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.Local.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddInMemoryCollection(new Dictionary<string, string>())
                    .Build();

            configuration["MigrateOnStartup"] = "true";

            string databaseServer = configuration.GetConnectionString("TestDatabaseServer");
            string databaseName = "LibraryTestDb_" + Guid.NewGuid();
            string databaseConnectionString = $"{databaseServer}Database={databaseName};";
            configuration["ConnectionStrings:Database"] = databaseConnectionString;

            return SetupServices(configuration, configure);
        }
    }
}