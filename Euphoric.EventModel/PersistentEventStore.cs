using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    public class PersistentEventStore : IEventStore, IDisposable
    {
        private DomainEventSender _sender;
        private EventStoreClient _client;
        private DomainEventFactory _eventFactory;
        private ILogger _logger;

        public PersistentEventStore(
            ILoggerFactory loggerFactory,
            DomainEventSender sender,
            DomainEventFactory eventFactory,
            IConfiguration configuration)
        {
            _sender = sender;
            _logger = loggerFactory.CreateLogger<PersistentEventStore>();
            var connectionString = configuration.GetConnectionString("EventStore");
            if (connectionString == null)
            {
                throw new InvalidOperationException("Connection string 'EventStore' must be set.");
            }
            _client = CreateClientWithConnection(connectionString, loggerFactory);
            _eventFactory = eventFactory;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task SubscribeClient()
        {
            await _client.SubscribeToAllAsync(HandleNewEvent);
            _logger.LogInformation("Subscribed to events.");
        }

        private static EventStoreClient CreateClientWithConnection(string connectionString, ILoggerFactory loggerFactory)
        {
            var settings = EventStoreClientSettings.Create(connectionString);
            settings.LoggerFactory = loggerFactory;
            return new EventStoreClient(settings);
        }

        public async Task<IDomainEvent<IDomainEventData>> Store(ICreateEvent<IDomainEventData> newEvent)
        {
            var eventData = newEvent.Data;

            var dataJson = _eventFactory.SerializeToData(eventData);
            var data = Encoding.UTF8.GetBytes(dataJson);
            var metadata = Encoding.UTF8.GetBytes("{}");

            var eventId = Guid.NewGuid();
            var eventName = _eventFactory.EventName(eventData);
            var evt = new EventData(Uuid.FromGuid(eventId), eventName, data, metadata);
            var eventStream = eventData.GetAggregateKey();

            IWriteResult result;
            if (newEvent.IsNewAggregate)
            {
                result = await _client.AppendToStreamAsync(eventStream, StreamState.NoStream, new List<EventData>() { evt });
            }
            else if (newEvent.IsVersioned)
            {
                result = await _client.AppendToStreamAsync(eventStream, new StreamRevision(newEvent.Version), new List<EventData>() { evt });
            }
            else
            {
                result = await _client.AppendToStreamAsync(eventStream, StreamState.StreamExists, new List<EventData>() { evt });
            }
            _logger.LogDebug("Appended event {position}|{type}.", result.LogPosition, evt.Type);

            return _eventFactory.CreateEvent(result.NextExpectedStreamRevision.ToUInt64(), SystemClock.Instance.GetCurrentInstant(),eventData);
        }

        private Task HandleNewEvent(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken token)
        {
            if (evnt.Event.EventStreamId.StartsWith('$'))
            {
                // skip system events
                return Task.CompletedTask;
            }

            IDomainEvent<IDomainEventData> @event;
            try
            {
                @event = ParseEvent(evnt);
            }
            catch
            {
                // TODO: Properly handle this case
                _logger.LogWarning("Unknown event name: {eventName}", evnt.Event.EventType);
                return Task.CompletedTask;
            }
            _logger.LogDebug("Processed event {position}|{type}.", evnt.OriginalPosition, evnt.Event.EventType);

            _sender.SendEvent(@event);

            return Task.CompletedTask;
        }

        private IDomainEvent<IDomainEventData> ParseEvent(ResolvedEvent resolvedEvent)
        {
            EventRecord evnt = resolvedEvent.Event;
            var dataJson = Encoding.UTF8.GetString(evnt.Data.Span);
            Instant created = Instant.FromDateTimeUtc(evnt.Created);
            var @event = _eventFactory.DeserializeFromData(evnt.EventId.ToGuid(), evnt.EventNumber.ToUInt64(), evnt.EventType, created, dataJson);
            return @event;
        }

        public IAsyncEnumerable<IDomainEvent<IDomainEventData>> GetAggregateEvents(string aggregateKey)
        {
            var events = _client.ReadStreamAsync(Direction.Forwards, aggregateKey, StreamPosition.Start);

            return events.Select(ParseEvent);
        }
    }
}
