using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LibraryWebsite.Books
{
    public class BookApiTest
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public BookApiTest()
        {
            var builder = WebHost.CreateDefaultBuilder()
                            .UseEnvironment(Environments.Production)
                            .UseStartup<Startup>()
                            .ConfigureTestServices(TestingStartup.ConfigureServices)

                            // override default startup configuration for testing purposes
                            .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning).AddConsole())
                            ;

            _testServer = new TestServer(builder);
            _testServer.BaseAddress = new Uri("https://localhost/"); // use HTTPS for all requests
            _client = _testServer.CreateClient();
        }

        [Fact]
        public async Task Retrieves_empty_books()
        {
            var books = await _client.GetJsonAsync<Book[]>("api/book");
            Assert.Empty(books);
        }

        private class Book
        {
            public Guid Id { get; set; }

            public string Title { get; set; }
            public string Author { get; set; }
            public string Description { get; set; }

            public string Isbn13 { get; set; }
        }


        [Fact]
        public async Task Creates_and_retrieves_books()
        {
            Book bookToCreate = new Book { Title = "Title X", Author = "Author Y", Description = "Descr Z", Isbn13 = "ISBN 13" };
            var bookGuid = await _client.PostJsonAsync<Guid>("api/book", bookToCreate);

            var books = await _client.GetJsonAsync<Book[]>("api/book");
            var createdBook = Assert.Single(books);

            Assert.Equal(bookGuid, createdBook.Id);
            Assert.Equal(bookToCreate.Title, createdBook.Title);
            Assert.Equal(bookToCreate.Author, createdBook.Author);
            Assert.Equal(bookToCreate.Description, createdBook.Description);
            Assert.Equal(bookToCreate.Isbn13, createdBook.Isbn13);
        }

        [Fact]
        public async Task Creates_and_retrieves_a_book()
        {
            for (int i = 0; i < 3; i++)
            {
                Book bookToCreate = new Book { Title = "Title " + i, Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN " + i };
                await _client.PostJsonAsync<Guid>("api/book", bookToCreate);
            }

            {
                Book bookToCreate = new Book { Title = "Title X", Author = "Author Y", Description = "Descr Z", Isbn13 = "ISBN 13" };
                Guid bookGuid = await _client.PostJsonAsync<Guid>("api/book", bookToCreate);

                var createdBook = await _client.GetJsonAsync<Book>("api/book/" + bookGuid);

                Assert.Equal(bookGuid, createdBook.Id);
                Assert.Equal(bookToCreate.Title, createdBook.Title);
                Assert.Equal(bookToCreate.Author, createdBook.Author);
                Assert.Equal(bookToCreate.Description, createdBook.Description);
                Assert.Equal(bookToCreate.Isbn13, createdBook.Isbn13);
            }
        }
    }
}
