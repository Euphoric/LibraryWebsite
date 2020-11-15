using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euphoric.EventModel;
using NodaTime;
using Xunit;

namespace LibraryWebsite.Books
{
    public class BookEventsTest
    {
        private readonly IEventStore _eventStore;

        public BookEventsTest()
        {
            var clock = new NodaTime.Testing.FakeClock(Instant.FromUtc(2020, 01, 01, 01, 01, 01));
            var domainEventFactory = new DomainEventFactory(new EventTypeLocator(typeof(BookDomainEvent).Assembly));
            _eventStore = new InMemoryEventStore(new DomainEventSender(new List<IDomainEventListener>()), domainEventFactory, clock);       
        }

        [Fact]
        public async Task Creates_book_aggregate()
        {
            var evnt = BookAggregate.New("Title X", "Author X", "ISBN13 X", "Description X");
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new [] { storedEvent });

            Assert.Equal("Title X", aggregate.Title);
            Assert.Equal("Author X", aggregate.Author);
            Assert.Equal("ISBN13 X", aggregate.Isbn13);
            Assert.Equal("Description X", aggregate.Description);
        }

        [Fact]
        public async Task Changes_book_aggregate()
        {
            var evnt = BookAggregate.New("Title X", "Author X", "ISBN13 X", "Description X");
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new [] { storedEvent });

            var evnt2 = aggregate.Change("Title Y", "Author Y", "ISBN13 Y", "Description Y");
            var storedEvent2 = await _eventStore.Store(evnt2);
            var aggregate2 = AggregateBuilder<BookAggregate>.Update(aggregate, new [] { storedEvent2 });

            Assert.Equal("Title Y", aggregate2.Title);
            Assert.Equal("Author Y", aggregate2.Author);
            Assert.Equal("ISBN13 Y", aggregate2.Isbn13);
            Assert.Equal("Description Y", aggregate2.Description);
        }

        [Fact]
        public async Task Deletes_book_aggregate()
        {
            var evnt = BookAggregate.New("Title X", "Author X", "ISBN13 X", "Description X");
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new [] { storedEvent });

            Assert.False(aggregate.IsDeleted);

            var evnt2 = aggregate.Delete();
            var storedEvent2 = await _eventStore.Store(evnt2);
            var aggregate2 = AggregateBuilder<BookAggregate>.Update(aggregate, new [] { storedEvent2 });

            Assert.True(aggregate2.IsDeleted);
        }

        [Fact]
        public async Task Cannot_change_when_deleted()
        {
            var evnt = BookAggregate.New("Title X", "Author X", "ISBN13 X", "Description X");
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new [] { storedEvent });

            Assert.False(aggregate.IsDeleted);

            var evnt2 = aggregate.Delete();
            var storedEvent2 = await _eventStore.Store(evnt2);
            var aggregate2 = AggregateBuilder<BookAggregate>.Update(aggregate, new [] { storedEvent2 });

            Assert.Throws<AggregateDeletedException>(()=>aggregate2.Change("Title Y", "Author Y", "ISBN13 Y", "Description Y"));
        }
    }
}
