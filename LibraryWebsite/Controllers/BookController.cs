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

            internal void SetupDeveloperData()
            {
                Books.Add(new Book { Id = Guid.NewGuid(), Title = "The Very Hungry Caterpillar", Author = "Eric Carle", Description = "THE all-time classic picture book, from generation to generation, sold somewhere in the world every 30 seconds! Have you shared it with a child or grandchild in your life?", Isbn13 = "978-0399226908" });
                Books.Add(new Book { Id = Guid.NewGuid(), Title = "The Hobbit", Author = "J.R.R. Tolkien", Description = "Bilbo Baggins is a hobbit who enjoys a comfortable, unambitious life, rarely traveling any farther than his pantry or cellar. But his contentment is disturbed when the wizard Gandalf and a company of dwarves arrive on his doorstep one day to whisk him away on an adventure. They have launched a plot to raid the treasure hoard guarded by Smaug the Magnificent, a large and very dangerous dragon. Bilbo reluctantly joins their quest, unaware that on his journey to the Lonely Mountain he will encounter both a magic ring and a frightening creature known as Gollum.", Isbn13 = "978-0547928227" });
                Books.Add(new Book { Id = Guid.NewGuid(), Title = "The Fellowship of the Ring: Being the First Part of The Lord of the Rings", Author = "J.R.R. Tolkien", Description = "One Ring to rule them all, One Ring to find them, One Ring to bring them all and in the darkness bind them In ancient times the Rings of Power were crafted by the Elven - smiths, and Sauron, the Dark Lord, forged the One Ring, filling it with his own power so that he could rule all others.But the One Ring was taken from him, and though he sought it throughout Middle - earth, it remained lost to him.After many ages it fell into the hands of Bilbo Baggins, as told in The Hobbit.In a sleepy village in the Shire, young Frodo Baggins finds himself faced with an immense task, as his elderly cousin Bilbo entrusts the Ring to his care. Frodo must leave his home and make a perilous journey across Middle - earth to the Cracks of Doom, there to destroy the Ring and foil the Dark Lord in his evil purpose.", Isbn13 = "978-0547928210" });
            }
        }

        private readonly Repository _repository;
        private readonly LibraryContext _context;

        public BookController(Repository repository, LibraryContext context)
        {
            _repository = repository;
            _context = context;
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
    }
}
