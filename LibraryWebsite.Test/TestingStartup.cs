using Microsoft.Extensions.DependencyInjection;
using System;
using Euphoric.EventModel;
using NodaTime;
using NodaTime.Testing;

namespace LibraryWebsite
{
    public static class TestingStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<IProjectionContainerFactory, SynchronousProjectionContainerFactory>();

            services.AddSingleton<IClock>(new FakeClock(Instant.FromUtc(2020, 2, 3, 4, 5)));
        }
    }
}
