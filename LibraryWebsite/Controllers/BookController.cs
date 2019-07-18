using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebsite.Controllers
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        [HttpGet]
        public IEnumerable<Book> Books()
        {
            return new Book[]
            {
                new Book{Id = Guid.NewGuid(), Title = "Title A", Author = "Author A", Description = "Descr 1", IBAN = "IBAN"}
            };
        }

        public class Book
        {
            public Guid Id { get; set; }

            public string Title { get; set; }
            public string Author { get; set; }
            public string Description { get; set; }
            public string IBAN { get; set; }
        }
    }
}
