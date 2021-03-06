using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebsite.Books
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly LibraryContext _context;

        public BookController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<BookDto>> Get()
        {
            return await _context.Books.Select(bk => ToDto(bk)).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<BookDto> Get(Guid id)
        {
            var book = await _context.Books.SingleOrDefaultAsync(bk => bk.Id == id);
            return ToDto(book);
        }

        [HttpGet("page")]
        public async Task<PagingResult<BookDto>> GetPaginated([FromQuery] int limit = 10, int page = 0)
        {
            return await _context.Books.OrderBy(x => x.Title).CreatePaging(limit, page).Select(ToDto);
        }

        [Authorize(Policy = Policies.CanEditBooks)]
        public async Task<Guid> Post([FromBody] BookDto book)
        {
            book.Id = Guid.NewGuid();
            await _context.Books.AddAsync(FromDto(book));
            await _context.SaveChangesAsync();

            return book.Id;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.CanEditBooks)]
        public async Task<ActionResult> Put(Guid id, [FromBody] BookDto book)
        {
            var bookToUpdate = await _context.Books.FirstOrDefaultAsync(bk => bk.Id == id);
            if (bookToUpdate == null)
            {
                return NotFound(id);
            }

            bookToUpdate.Title = book.Title;
            bookToUpdate.Author = book.Author;
            bookToUpdate.Description = book.Description;
            bookToUpdate.Isbn13 = book.Isbn13;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.CanEditBooks)]
        public async Task<ActionResult> Delete(Guid id)
        {
            // workaround around broken FirstOrDefault in EFInMemory 3.0-preview7
            var bookToDelete = (await _context.Books.Where(bk => bk.Id == id).Take(1).ToListAsync()).FirstOrDefault();
            if (bookToDelete == null)
            {
                return Ok();
            }

            _context.Books.Remove(bookToDelete);

            await _context.SaveChangesAsync();

            return Ok();
        }

        private static BookDto ToDto(Book bk)
        {
            return new BookDto
            {
                Id = bk.Id,
                Title = bk.Title,
                Isbn13 = bk.Isbn13,
                Author = bk.Author,
                Description = bk.Description
            };
        }

        private static Book FromDto(BookDto dto)
        {
            return new Book
            {
                Id = dto.Id,
                Title = dto.Title,
                Isbn13 = dto.Isbn13,
                Author = dto.Author,
                Description = dto.Description
            };
        }
    }
}
