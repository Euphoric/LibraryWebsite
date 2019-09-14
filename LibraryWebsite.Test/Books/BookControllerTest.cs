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

        BookController controller;

        public BookControllerTest()
        {
            LibraryContext dbContext = CreateDbContext();
            controller = new BookController(dbContext);
        }

        [Fact]
        public async Task Retrieves_empty_books()
        {
            Assert.Empty(await controller.Get());
        }

        [Fact]
        public async Task Creates_new_book()
        {
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

        const int defaultLimit = 10;

        [Fact]
        public async Task Book_pagination_limit_is_default()
        {
            // reverse order to test ordering
            for (int i = 0; i < 30; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            var books = (await controller.GetPaginated()).ToArray();
            Assert.Equal(defaultLimit, books.Length);

            var expectedTitles = Enumerable.Range(0, defaultLimit).Select(i => $"Title {i:D3}");
            Assert.Equal(expectedTitles, books.Select(x => x.Title));
        }

        [Fact]
        public async Task Book_pagination_limit_set()
        {
            // reverse order to test ordering
            for (int i = 0; i < 20; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            var limit = 7;
            var books = (await controller.GetPaginated(limit: limit)).ToArray();
            Assert.Equal(limit, books.Length);

            var expectedTitles = Enumerable.Range(0, limit).Select(i => $"Title {i:D3}");
            Assert.Equal(expectedTitles, books.Select(x => x.Title));
        }

        [Fact]
        public async Task Book_pagination_page()
        {
            // reverse order to test ordering
            for (int i = 0; i < 30; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            var page = 1;
            var books = (await controller.GetPaginated(page: page)).ToArray();
            Assert.Equal(defaultLimit, books.Length);

            var expectedTitles = Enumerable.Range(defaultLimit * page, defaultLimit).Select(i => $"Title {i:D3}");
            Assert.Equal(expectedTitles, books.Select(x => x.Title));

            // TODO
        }
    }
}
