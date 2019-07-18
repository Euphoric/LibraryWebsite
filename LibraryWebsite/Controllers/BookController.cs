using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebsite.Controllers
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        public class Repository
        {
            public Collection<Book> Books { get; } = new Collection<Book>();
        }

        private readonly Repository _repository;

        public BookController(Repository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Book> Books()
        {
            return _repository.Books;
        }

        public void Post([FromBody]Book book)
        {
            book.Id = Guid.NewGuid();
            _repository.Books.Add(book);
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
