using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euphoric.EventModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebsite.Books
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IEventStore _eventStore;
        private readonly IProjectionState<BooksListProjection> _bookList;

        public BookController(IEventStore eventStore, IProjectionState<BooksListProjection> bookList)
        {
            _eventStore = eventStore;
            _bookList = bookList;
        }

        [HttpGet]
        public async Task<IEnumerable<BookDto>> Get()
        {
            await Task.Yield();

            return _bookList.State.ListBooks().Select(ToDto);
        }

        [HttpGet("{id}")]
        public async Task<BookDto> Get(EntityId id)
        {
            await Task.Yield();
            var book = _bookList.State.ListBooks().Single(bk => bk.Id == id);
            return ToDto(book);
        }

        [HttpGet("page")]
        public async Task<PagingResult<BookDto>> GetPaginated([FromQuery] int limit = 10, int page = 0)
        {
            return await _bookList.State.ListBooks().ToAsyncEnumerable().OrderBy(x => x.Title).CreatePaging(limit, page).Select(ToDto);
        }

        [Authorize(Policy = Policies.CanEditBooks)]
        public async Task<EntityId> Post([FromBody] BookDto book)
        {
            var evnt = BookAggregate.New(book.Title!, book.Author!, book.Isbn13!, book.Description!);
            var storedEvent = await _eventStore.Store(evnt);
            var bookAggregate = AggregateBuilder<BookAggregate>.Rehydrate(new[] { storedEvent });

            return bookAggregate.Id;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.CanEditBooks)]
        public async Task<ActionResult> Put(EntityId id, [FromBody] BookDto bookDto)
        {
            var book = await _eventStore.RetrieveAggregate(BookKey.FromEntityId(id));
            if (book == null)
            {
                return NotFound(id);
            }

            var evnt = book.Change(bookDto.Title!, bookDto.Author!, bookDto.Isbn13!, bookDto.Description!);
            await _eventStore.Store(evnt);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.CanEditBooks)]
        public async Task<ActionResult> Delete(EntityId id)
        {
            var bookToDelete = await _eventStore.RetrieveAggregate(BookKey.FromEntityId(id));
            if (bookToDelete == null)
            {
                return Ok();
            }

            // TODO: check if the item is deleted

            var evnt = bookToDelete.Delete();
            await _eventStore.Store(evnt);

            return Ok();
        }

        private static BookDto ToDto(BooksListProjection.Book bk)
        {
            return new BookDto
            {
                Id = new EntityId(bk.Id.ToString()),
                Title = bk.Title,
                Isbn13 = bk.Isbn13,
                Author = bk.Author,
                Description = bk.Description
            };
        }
    }
}
