using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Euphoric.EventModel;

namespace LibraryWebsite.Books
{
    public record BookKey(Guid id) : IAggregateKey<BookAggregate, BookDomainEvent>
    {
        public static PrefixedGuidKey<BookKey> Format { get; } = new PrefixedGuidKey<BookKey>("book");

        public string Value => Format.ToValue(id);

        public static implicit operator EntityId(BookKey key) => new EntityId(key.Value);
        public static implicit operator BookKey(EntityId id) => Format.Parse(id.Value);

        public static EntityId ToEntityId(BookKey key) => new EntityId(key.Value);
        public static BookKey FromEntityId(EntityId id) => Format.Parse(id.Value);
    }

    public record BookAggregate(BookKey Id, string Title, string Author, string Isbn13, string Description, bool IsDeleted) : Aggregate
    {
        #region Rehydrate

        public static BookAggregate Initialize(IDomainEvent<BookCreated> createdEvent)
        {
            var eventData = createdEvent.Data;
            return new BookAggregate(eventData.Id, eventData.Title, eventData.Author, eventData.Isbn13, eventData.Description, false);
        }

        public BookAggregate Update(IDomainEvent<BookDomainEvent> evnt)
        {
            switch (evnt.Data)
            {
                case BookDeleted:
                    return this with { IsDeleted = true };
                case BookChanged eventData:
                    return this with
                    {
                        Title = eventData.Title,
                        Author = eventData.Author,
                        Isbn13 =
                        eventData.Isbn13,
                        Description = eventData.Description
                    };
                default:
                    throw new NotSupportedException("Unknown event type.");
            }
        }

        #endregion

        #region Modify

        public static ICreateEvent<BookDomainEvent> New(string title, string author, string isbn13, string description)
        {
            BookKey newId = BookKey.Format.New();
            return new BookCreated(newId, title, author, isbn13, description).AsNewAggregate();
        }

        public ICreateEvent<BookDomainEvent> Change(string title, string author, string isbn13, string description)
        {
            if (IsDeleted)
            {
                throw new AggregateDeletedException($"Book {Id} is deleted.");
            }

            return new BookChanged(Id, title, author, isbn13, description).AsVersioned(Version);
        }

        public ICreateEvent<BookDomainEvent> Delete()
        {
            return new BookDeleted(Id).AsVersioned(Version);
        }

        #endregion
    }

    public sealed record BooksListProjection : IProjection
    {
        public sealed record Book(EntityId Id, string Title, string Author, string Isbn13, string Description);

        ImmutableDictionary<BookKey, Book> Books { get; init; } = ImmutableDictionary<BookKey, Book>.Empty;

        public IEnumerable<Book> ListBooks()
        {
            return Books.Values;
        }

        public IProjection NextState(IDomainEvent<IDomainEventData> evnt)
        {
            switch (evnt.Data)
            {
                case BookCreated created:
                    var newItem = new Book(created.Id, created.Title, created.Author, created.Isbn13, created.Description);
                    return this with { Books = Books.Add(created.Id, newItem) };
                case BookChanged changed:
                    var book = Books[changed.Id];
                    book = book with
                        {
                        Title = changed.Title,
                        Author = changed.Author,
                        Isbn13 = changed.Isbn13,
                        Description = changed.Description
                        };
                    return this with { Books = Books.SetItem(changed.Id, book)};

                case BookDeleted deleted:
                    return this with { Books = Books.Remove(deleted.Id) };
                default:
                    return this;
            }
        }
    }

    public abstract record BookDomainEvent(BookKey Id) : IDomainEventData
    {
        public string GetAggregateKey()
        {
            return Id.Value;
        }
    }

    [DomainEvent("book-created")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Record bug")]
    public record BookCreated(BookKey Id, string Title, string Author, string Isbn13, string Description) : BookDomainEvent(Id)
    {
    }

    [DomainEvent("book-changed")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Record bug")]
    public record BookChanged(BookKey Id, string Title, string Author, string Isbn13, string Description) : BookDomainEvent(Id)
    {
    }

    [DomainEvent("book-deleted")]
    public record BookDeleted(BookKey Id) : BookDomainEvent(Id)
    {
    }

    public class AggregateDeletedException : Exception
    {
        public AggregateDeletedException()
        {
        }

        public AggregateDeletedException(string message) : base(message)
        {
        }

        public AggregateDeletedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
