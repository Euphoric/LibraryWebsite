using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LibraryWebsite.Books
{
    public class BookControllerTest
    {
        private static LibraryContext CreateDbContext()
        {
            DbContextOptionsBuilder<LibraryContext> builder = new DbContextOptionsBuilder<LibraryContext>();
            var dbName = Guid.NewGuid().ToString();
            builder.UseInMemoryDatabase(dbName);
            return new LibraryContext(builder.Options);
        }

        [Fact]
        public async Task Retrieves_empty_books()
        {
            LibraryContext dbContext = CreateDbContext();
            var controller = new BookController(dbContext);

            Assert.Empty(await controller.Get());
        }

        [Fact]
        public async Task Creates_new_book()
        {
            LibraryContext dbContext = CreateDbContext();
            var controller = new BookController(dbContext);

            Book bookToCreate = new Book() { Title = "Title X", Author = "Author Y", Description = "Descr Z", Isbn13 = "IBANQ" };
            await controller.Post(bookToCreate);

            IEnumerable<Book> books = (await controller.Get()).ToList();
            var createdBook = Assert.Single(books);

            Assert.NotEqual(Guid.Empty, createdBook.Id);
            Assert.Equal(bookToCreate.Title, createdBook.Title);
            Assert.Equal(bookToCreate.Author, createdBook.Author);
            Assert.Equal(bookToCreate.Description, createdBook.Description);
            Assert.Equal(bookToCreate.Isbn13, createdBook.Isbn13);
        }
    }
}
