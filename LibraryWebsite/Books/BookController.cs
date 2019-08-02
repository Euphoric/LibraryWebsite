using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebsite.Books
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly LibraryContext _context;

        public BookController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Book>> Get()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Book> Get(Guid id)
        {
            return await _context.Books.SingleOrDefaultAsync(bk => bk.Id == id);
        }

        public async Task<Guid> Post([FromBody]Book book)
        {
            book.Id = Guid.NewGuid();
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book.Id;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody]Book book)
        {
            // workaround around broken FirstOrDefault in EFInMemory 3.0-preview7
            var bookToUpdate = (await _context.Books.Where(bk => bk.Id == id).Take(1).ToListAsync()).FirstOrDefault();
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
    }
}
