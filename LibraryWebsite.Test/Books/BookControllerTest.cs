using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;
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

            var operationalStoreOptions = Options.Create(new OperationalStoreOptions());

            return new LibraryContext(builder.Options, operationalStoreOptions);
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

        const int DefaultLimit = 10;

        [Fact]
        public async Task Book_pagination_limit_is_default()
        {
            for (int i = 0; i < 30; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            var books = (await controller.GetPaginated()).Items;
            Assert.Equal(DefaultLimit, books.Length);

            var expectedTitles = Enumerable.Range(0, DefaultLimit).Select(i => $"Title {i:D3}");
            Assert.Equal(expectedTitles, books.Select(x => x.Title));
        }

        [Fact]
        public async Task Book_pagination_limit_set()
        {
            for (int i = 0; i < 20; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            var limit = 7;
            var result = await controller.GetPaginated(limit: limit);
            var books = result.Items;
            Assert.Equal(limit, books.Length);

            var expectedTitles = Enumerable.Range(0, limit).Select(i => $"Title {i:D3}");
            Assert.Equal(expectedTitles, books.Select(x => x.Title));

            Assert.Equal(0, result.CurrentPage);
        }

        [Fact]
        public async Task Book_pagination_page()
        {
            for (int i = 0; i < 30; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            var page = 1;
            var result = await controller.GetPaginated(page: page);
            var books = result.Items;
            Assert.Equal(DefaultLimit, books.Length);

            var expectedTitles = Enumerable.Range(DefaultLimit * page, DefaultLimit).Select(i => $"Title {i:D3}");
            Assert.Equal(expectedTitles, books.Select(x => x.Title));

            Assert.Equal(page, result.CurrentPage);
        }

        [Fact]
        public async Task Book_pagination_total_pages()
        {
            {
                var result = await controller.GetPaginated();
                Assert.Equal(0, result.TotalPages);
                Assert.Equal(0, result.TotalCount);
            }

            for (int i = 0; i < 10; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            {
                var result = await controller.GetPaginated();
                Assert.Equal(1, result.TotalPages);
                Assert.Equal(10, result.TotalCount);
            }

            for (int i = 0; i < 1; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            {
                var result = await controller.GetPaginated();
                Assert.Equal(2, result.TotalPages);
                Assert.Equal(11, result.TotalCount);
            }

            {
                var result = await controller.GetPaginated(limit: 11);
                Assert.Equal(1, result.TotalPages);
                Assert.Equal(11, result.TotalCount);
            }

            for (int i = 0; i < 10; i++)
            {
                Book bookToCreate = new Book { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await controller.Post(bookToCreate);
            }

            {
                var result = await controller.GetPaginated();
                Assert.Equal(3, result.TotalPages);
                Assert.Equal(21, result.TotalCount);
            }

            {
                var result = await controller.GetPaginated(limit: 21);
                Assert.Equal(1, result.TotalPages);
                Assert.Equal(21, result.TotalCount);
            }
        }
    }
}
