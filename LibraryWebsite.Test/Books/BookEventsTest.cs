using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euphoric.EventModel;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Xunit;

namespace LibraryWebsite.Books
{
    public class BookEventsTest
    {
        private readonly IEventStore _eventStore;
        private readonly IProjectionState<BooksListProjection> _listProjection;

        private static void AddTestEventServices(ServiceCollection services)
        {
            var clock = new NodaTime.Testing.FakeClock(Instant.FromUtc(2020, 01, 01, 01, 01, 01));
            services.AddSingleton<IClock>(clock);

            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<DomainEventSender>();
            services.AddSingleton<DomainEventFactory>();
            services.AddSingleton(new EventTypeLocator(typeof(BookDomainEvent).Assembly));

            services.AddSingleton<IProjectionContainerFactory, SynchronousProjectionContainerFactory>();
        }

        private static void AddProjection<TProjection>(ServiceCollection services)
        {
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionState<BooksListProjection>());
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionListener<BooksListProjection>());
        }

        public BookEventsTest()
        {
            var services = new ServiceCollection();

            AddTestEventServices(services);
            AddProjection<BooksListProjection>(services);
            var provider = services.BuildServiceProvider();

            _eventStore = provider.GetRequiredService<IEventStore>();
            _listProjection = provider.GetRequiredService<IProjectionState<BooksListProjection>>();
        }

        [Fact]
        public void List_projection_with_no_books()
        {
            Assert.Empty(_listProjection.State.ListBooks());
        }

        [Fact]
        public async Task Creates_book_aggregate()
        {
            var evnt = BookAggregate.New("Title X", "Author X", "ISBN13 X", "Description X");
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new[] { storedEvent });

            Assert.Equal("Title X", aggregate.Title);
            Assert.Equal("Author X", aggregate.Author);
            Assert.Equal("ISBN13 X", aggregate.Isbn13);
            Assert.Equal("Description X", aggregate.Description);

            var projectedBook = Assert.Single(_listProjection.State.ListBooks())!;
            Assert.Equal("Title X", projectedBook.Title);
            Assert.Equal("Author X", projectedBook.Author);
            Assert.Equal("ISBN13 X", projectedBook.Isbn13);
            Assert.Equal("Description X", projectedBook.Description);
        }

        [Fact]
        public async Task Changes_book_aggregate()
        {
            var evnt = BookAggregate.New("Title X", "Author X", "ISBN13 X", "Description X");
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new[] { storedEvent });

            var evnt2 = aggregate.Change("Title Y", "Author Y", "ISBN13 Y", "Description Y");
            var storedEvent2 = await _eventStore.Store(evnt2);
            var aggregate2 = AggregateBuilder<BookAggregate>.Update(aggregate, new[] { storedEvent2 });

            Assert.Equal("Title Y", aggregate2.Title);
            Assert.Equal("Author Y", aggregate2.Author);
            Assert.Equal("ISBN13 Y", aggregate2.Isbn13);
            Assert.Equal("Description Y", aggregate2.Description);

            var projectedBook = Assert.Single(_listProjection.State.ListBooks())!;
            Assert.Equal("Title Y", projectedBook.Title);
            Assert.Equal("Author Y", projectedBook.Author);
            Assert.Equal("ISBN13 Y", projectedBook.Isbn13);
            Assert.Equal("Description Y", projectedBook.Description);
        }

        [Fact]
        public async Task Deletes_book_aggregate()
        {
            var evnt = BookAggregate.New("Title X", "Author X", "ISBN13 X", "Description X");
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new[] { storedEvent });

            Assert.False(aggregate.IsDeleted);

            var evnt2 = aggregate.Delete();
            var storedEvent2 = await _eventStore.Store(evnt2);
            var aggregate2 = AggregateBuilder<BookAggregate>.Update(aggregate, new[] { storedEvent2 });

            Assert.True(aggregate2.IsDeleted);

            Assert.Empty(_listProjection.State.ListBooks());
        }

        [Fact]
        public async Task Cannot_change_when_deleted()
        {
            var evnt = BookAggregate.New("Title X", "Author X", "ISBN13 X", "Description X");
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new[] { storedEvent });

            Assert.False(aggregate.IsDeleted);

            var evnt2 = aggregate.Delete();
            var storedEvent2 = await _eventStore.Store(evnt2);
            var aggregate2 = AggregateBuilder<BookAggregate>.Update(aggregate, new[] { storedEvent2 });

            Assert.Throws<AggregateDeletedException>(() => aggregate2.Change("Title Y", "Author Y", "ISBN13 Y", "Description Y"));
        }

        [Fact]
        public async Task Multiple_books_in_projection()
        {
            for (int i = 0; i < 10; i++)
            {
                var evnt = BookAggregate.New("Title " + i, "Author X", "ISBN13 X", "Description X");
                var storedEvent = await _eventStore.Store(evnt);
                var aggregate = AggregateBuilder<BookAggregate>.Rehydrate(new[] { storedEvent });
            }

            var bookTitles = _listProjection.State.ListBooks().Select(x => x.Title);
            Assert.Equal(bookTitles.OrderBy(x=>x), Enumerable.Range(0, 10).Select(x => "Title " + x));
        }
    }
}
