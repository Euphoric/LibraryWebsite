using NodaTime;
using System;

namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventData
    {
        /// <summary>
        /// Returns domain's aggregate key.
        /// </summary>
        /// <remarks>
        /// Keep as method so it is not serialized.
        /// </remarks>
        string GetAggregateKey();
    }

    public interface IDomainEvent<out TEvent>
        where TEvent : IDomainEventData
    {
        Guid Id { get; }
        ulong Version { get; }
        string AggregateKey { get; }
        string EventName { get; }
        Instant Created { get; }
        TEvent Data { get; }
    }

    public interface ICreateEvent<out TEvent>
        where TEvent : IDomainEventData
    {
        bool IsNewAggregate { get; }
        bool IsVersioned { get; }
        ulong Version { get; }
        TEvent Data { get; }
    }

    public static class CreateEventExtension
    {
        private record CreateEvent<TEvent>(TEvent Data, bool IsNewAggregate, bool IsVersioned, ulong Version) : ICreateEvent<TEvent>
            where TEvent : IDomainEventData;

        public static ICreateEvent<TEvent> AsNewAggregate<TEvent>(this TEvent data)
            where TEvent : IDomainEventData
        {
            return new CreateEvent<TEvent>(data, true, false, 0);
        }

        public static ICreateEvent<TEvent> AsVersioned<TEvent>(this TEvent data, ulong version)
            where TEvent : IDomainEventData
        {
            return new CreateEvent<TEvent>(data, false, true, version);
        }

        public static ICreateEvent<TEvent> AsUnversioned<TEvent>(this TEvent data)
            where TEvent : IDomainEventData
        {
            return new CreateEvent<TEvent>(data, false, false, 0);
        }
    }
}
