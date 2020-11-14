using NodaTime;
using System;
using System.Text.Json;

namespace BlazorEventsTodo.EventStorage
{
    public class DomainEventFactory
    {
        private readonly EventTypeLocator _eventTypeLocator;

        public DomainEventFactory()
        {
            _eventTypeLocator = new EventTypeLocator();
        }

        private class DomainEvent<TData> : IDomainEvent<TData>
            where TData : IDomainEventData
        {
            public DomainEvent(Guid id, ulong version, TData data, string eventName, Instant created)
            {
                Id = id;
                Version = version;
                Data = data;
                EventName = eventName;
                Created = created;
            }

            public Guid Id { get; }
            public ulong Version { get; }
            public TData Data { get; }
            public string EventName { get; }
            public Instant Created { get; }

            public string AggregateKey => Data.GetAggregateKey();
        }

        public IDomainEvent<IDomainEventData> DeserializeFromData(Guid id, ulong version, string eventName, Instant created, string dataJson)
        {
            var eventType = _eventTypeLocator.GetClrType(eventName);

            if (eventType == null)
            {
                throw new Exception("Unknown event name: " + eventName);
            }

            var data = JsonSerializer.Deserialize(dataJson, eventType);

            var domainEventContainerType = typeof(DomainEvent<>).MakeGenericType(eventType);
            return (IDomainEvent<IDomainEventData>)Activator.CreateInstance(domainEventContainerType, args: new object[] { id, version, data, eventName, created });
        }

        public IDomainEvent<IDomainEventData> CreateEvent(ulong version, Instant created, IDomainEventData eventData)
        {
            var eventType = eventData.GetType();
            var id = Guid.NewGuid();
            var eventName = EventName(eventData);
            var domainEventContainerType = typeof(DomainEvent<>).MakeGenericType(eventType);
            return (IDomainEvent<IDomainEventData>)Activator.CreateInstance(domainEventContainerType, args: new object[] { id, version, eventData, eventName, created });
        }

        public string EventName(IDomainEventData eventData)
        {
            return _eventTypeLocator.GetTypeString(eventData.GetType());
        }

        public string SerializeToData(IDomainEventData eventData)
        {
            var eventType = eventData.GetType();
            return JsonSerializer.Serialize(eventData, eventType);
        }
    }
}
