using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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