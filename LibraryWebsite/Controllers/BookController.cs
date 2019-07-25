using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebsite.Controllers
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
        public async Task<IEnumerable<Book>> Books()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task Post([FromBody]Book book)
        {
            book.Id = Guid.NewGuid();
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }
    }
}
