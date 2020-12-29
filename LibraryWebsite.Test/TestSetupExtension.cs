using System;
using System.Collections.Generic;
using Euphoric.EventModel;
using LibraryWebsite.Books;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace LibraryWebsite
{
    public static class TestSetupExtension
    {
        public static IServiceCollection AddTestEventServices(this IServiceCollection services)
        {
            var clock = new NodaTime.Testing.FakeClock(Instant.FromUtc(2020, 01, 01, 01, 01, 01));
            services.AddSingleton<IClock>(clock);

            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<DomainEventSender>();
            services.AddSingleton<DomainEventFactory>();
            services.AddSingleton(new EventTypeLocator(typeof(BookDomainEvent).Assembly));

            services.AddSingleton<IProjectionContainerFactory, SynchronousProjectionContainerFactory>();

            var configuration =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>())
                    .Build();

            services.AddSingleton<IConfiguration>(configuration);

            return services;
        }

        public static IServiceCollection AddProjection<TProjection>(this IServiceCollection services)
            where TProjection : IProjection, new()
        {
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionState<TProjection>());
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionListener<TProjection>());

            return services;
        }
    }
}