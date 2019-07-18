using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LibraryWebsite.Controllers
{
    public class BookControllerTest
    {
        [Fact]
        public void Retrieves_empty_books()
        {
            var controller = new BookController(new BookController.Repository());
            Assert.Empty(controller.Books());
        }

        [Fact]
        public void Creates_new_book()
        {
            var controller = new BookController(new BookController.Repository());
            BookController.Book bookToCreate = new BookController.Book() { Title = "Title X", Author = "Author Y", Description = "Descr Z", IBAN = "IBANQ" };
            controller.Post(bookToCreate);

            IEnumerable<BookController.Book> books = controller.Books().ToList();
            var createdBook = Assert.Single(books);

            Assert.NotEqual(Guid.Empty, createdBook.Id);
            Assert.Equal(bookToCreate.Title, createdBook.Title);
            Assert.Equal(bookToCreate.Author, createdBook.Author);
            Assert.Equal(bookToCreate.Description, createdBook.Description);
            Assert.Equal(bookToCreate.IBAN, createdBook.IBAN);
        }
    }
}
