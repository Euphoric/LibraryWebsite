using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using Euphoric.EventModel;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using NodaTime.Testing;

namespace LibraryWebsite
{
    public static class TestingStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.RemoveAll<DbContextOptions<LibraryContext>>();
            var dbName = Guid.NewGuid().ToString();
            services.AddDbContext<LibraryContext>(options => options.UseInMemoryDatabase(dbName));

            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<IProjectionContainerFactory, SynchronousProjectionContainerFactory>();

            services.AddSingleton<IClock>(new FakeClock(Instant.FromUtc(2020, 2, 3, 4, 5)));
        }
    }
}
