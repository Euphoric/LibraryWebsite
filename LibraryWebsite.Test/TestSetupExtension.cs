﻿using System;
using Euphoric.EventModel;
using LibraryWebsite.Books;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace LibraryWebsite
{
    public static class TestSetupExtension
    {
        public static ServiceCollection AddTestEventServices(this ServiceCollection services)
        {
            var clock = new NodaTime.Testing.FakeClock(Instant.FromUtc(2020, 01, 01, 01, 01, 01));
            services.AddSingleton<IClock>(clock);

            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<DomainEventSender>();
            services.AddSingleton<DomainEventFactory>();
            services.AddSingleton(new EventTypeLocator(typeof(BookDomainEvent).Assembly));

            services.AddSingleton<IProjectionContainerFactory, SynchronousProjectionContainerFactory>();

            return services;
        }

        public static ServiceCollection AddProjection<TProjection>(this ServiceCollection services)
            where TProjection : IProjection, new()
        {
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionState<TProjection>());
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionListener<TProjection>());

            return services;
        }
    }
}