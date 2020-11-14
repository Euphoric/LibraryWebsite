using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    public interface IEventStore
    {
        public Task<IDomainEvent<IDomainEventData>> Store(ICreateEvent<IDomainEventData> eventData);

        IAsyncEnumerable<IDomainEvent<IDomainEventData>> GetAggregateEvents(string aggregateKey);
    }

    public static class EventStoreExtensions
    {
        public static IAsyncEnumerable<IDomainEvent<TEvent>> GetAggregateEvents<TAggregate, TEvent>(this IEventStore eventStore, IAggregateKey<TAggregate, TEvent> aggregateKey)
            where TAggregate : Aggregate
            where TEvent : IDomainEventData
        {
            return eventStore.GetAggregateEvents(aggregateKey.Value).Cast<IDomainEvent<TEvent>>();
        }

        public static async Task<TAggregate> RetrieveAggregate<TAggregate, TEvent>(this IEventStore eventStore, IAggregateKey<TAggregate, TEvent> aggregateKey)
            where TAggregate : Aggregate
            where TEvent : IDomainEventData
        {
            return AggregateBuilder<TAggregate>.Rehydrate(await eventStore.GetAggregateEvents(aggregateKey.Value).ToListAsync());
        }
    }

    public class EventStore : IEventStore
    {
        private readonly List<IDomainEvent<IDomainEventData>> _events = new List<IDomainEvent<IDomainEventData>>();
        private readonly DomainEventSender _sender;
        private readonly DomainEventFactory _eventFactory;
        private readonly IClock _clock;

        public EventStore(DomainEventSender sender, DomainEventFactory eventFactory, IClock clock)
        {
            _sender = sender;
            _eventFactory = eventFactory;
            _clock = clock;
        }

        public IAsyncEnumerable<IDomainEvent<IDomainEventData>> GetAggregateEvents(string aggregateKey)
        {
            return _events.Where(x => x.AggregateKey == aggregateKey).ToAsyncEnumerable();
        }

        public Task<IDomainEvent<IDomainEventData>> Store(ICreateEvent<IDomainEventData> newEvent)
        {
            var eventData = newEvent.Data;
            var eventVersion = _events.Where(x => x.AggregateKey == eventData.GetAggregateKey()).Select(x => (ulong?)x.Version).Max(x => x) ?? 0;
            Instant created = _clock.GetCurrentInstant();
            var @event = _eventFactory.CreateEvent(eventVersion, created, eventData);

            _events.Add(@event);
            _sender.SendEvent(@event);

            var result = _eventFactory.CreateEvent(eventVersion, created, eventData);
            return Task.FromResult(result);
        }
    }
}
