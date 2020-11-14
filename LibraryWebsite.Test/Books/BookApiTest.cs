using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LibraryWebsite.Identity;
using Xunit;
using Xunit.Abstractions;

namespace LibraryWebsite.Books
{

    public class BookApiTest : IAsyncLifetime
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public BookApiTest(ITestOutputHelper outputHelper)
        {
            _testServer = TestServerCreator.CreateTestServer(outputHelper);
            _client = _testServer.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await _testServer.Host.Services.AddTestingUsers();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Retrieves_empty_books()
        {
            var books = await _client.GetJsonAsync<BookDto[]>("api/book");
            Assert.Empty(books);
        }

        [Fact]
        public async Task Creates_and_retrieves_books()
        {
            await _client.LoginAsLibrarian();

            BookDto bookToCreate = new BookDto { Title = "Title X", Author = "Author Y", Description = "Descr Z", Isbn13 = "ISBN 13" };
            var bookId = await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);

            await _client.LogoutUser();

            var books = await _client.GetJsonAsync<BookDto[]>("api/book");
            var createdBook = Assert.Single(books);

            Assert.Equal(bookId, createdBook.Id);
            Assert.Equal(bookToCreate.Title, createdBook.Title);
            Assert.Equal(bookToCreate.Author, createdBook.Author);
            Assert.Equal(bookToCreate.Description, createdBook.Description);
            Assert.Equal(bookToCreate.Isbn13, createdBook.Isbn13);
        }

        [Fact]
        public async Task Creates_and_retrieves_a_book()
        {
            await _client.LoginAsLibrarian();

            for (int i = 0; i < 3; i++)
            {
                BookDto bookToCreate = new BookDto { Title = "Title " + i, Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN " + i };
                await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);
            }

            {
                BookDto bookToCreate = new BookDto { Title = "Title X", Author = "Author Y", Description = "Descr Z", Isbn13 = "ISBN 13" };
                var bookId = await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);

                await _client.LogoutUser();

                var createdBook = await _client.GetJsonAsync<BookDto>("api/book/" + bookId);

                Assert.Equal(bookId, createdBook.Id);
                Assert.Equal(bookToCreate.Title, createdBook.Title);
                Assert.Equal(bookToCreate.Author, createdBook.Author);
                Assert.Equal(bookToCreate.Description, createdBook.Description);
                Assert.Equal(bookToCreate.Isbn13, createdBook.Isbn13);
            }
        }

        [Fact]
        public async Task Updating_nonexistent_book_is_error()
        {
            await _client.LoginAsLibrarian();

            Guid bookGuid = Guid.Parse("43864ebe-8507-440f-babf-eb38e91d252c");
            BookDto bookUpdate = new BookDto { Title = "Title W", Author = "Author G", Description = "Descr C", Isbn13 = "ISBN 987" };
            var result = await _client.PutJsonErrorResponseAsync("api/book/" + bookGuid, bookUpdate);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Updates_book()
        {
            await _client.LoginAsLibrarian();

            BookDto bookToCreate = new BookDto { Title = "Title X", Author = "Author Y", Description = "Descr Z", Isbn13 = "ISBN 13" };
            var bookId = await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);

            BookDto bookUpdate = new BookDto { Id = bookId, Title = "Title W", Author = "Author G", Description = "Descr C", Isbn13 = "ISBN 987" };
            await _client.PutJsonAsync("api/book/" + bookId, bookUpdate);

            await _client.LogoutUser();

            var books = await _client.GetJsonAsync<BookDto[]>("api/book");
            var createdBook = Assert.Single(books);

            Assert.Equal(bookId, createdBook.Id);
            Assert.Equal(bookUpdate.Title, createdBook.Title);
            Assert.Equal(bookUpdate.Author, createdBook.Author);
            Assert.Equal(bookUpdate.Description, createdBook.Description);
            Assert.Equal(bookUpdate.Isbn13, createdBook.Isbn13);
        }

        [Fact]
        public async Task Deletes_book()
        {
            await _client.LoginAsLibrarian();

            BookDto bookToCreate = new BookDto { Title = "Title X", Author = "Author Y", Description = "Descr Z", Isbn13 = "ISBN 13" };
            var bookGuid = await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);

            await _client.DeleteAsync("api/book/" + bookGuid);

            await _client.LogoutUser();

            var books = await _client.GetJsonAsync<BookDto[]>("api/book");
            Assert.Empty(books);
        }

        [Fact]
        public async Task Deletes_book_specified_by_id()
        {
            await _client.LoginAsLibrarian();

            List<EntityId> createdBookIds = new List<EntityId>();
            for (int i = 0; i < 3; i++)
            {
                BookDto bookToCreate = new BookDto { Title = "Title " + i, Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN " + i };
                var createdBookId = await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);
                createdBookIds.Add(createdBookId);
            }

            {
                BookDto bookToCreate = new BookDto { Title = "Title X", Author = "Author Y", Description = "Descr Z", Isbn13 = "ISBN 13" };
                var bookId = await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);

                await _client.DeleteAsync("api/book/" + bookId);

                await _client.LogoutUser();

                var bookIds = (await _client.GetJsonAsync<BookDto[]>("api/book")).Select(bk => bk.Id).ToArray();
                Assert.DoesNotContain(bookId, bookIds);
                Assert.Equal(createdBookIds, bookIds);
            }
        }

        [Fact]
        public async Task Deleting_non_existing_book_is_noop()
        {
            await _client.LoginAsLibrarian();

            Guid bookGuid = Guid.Parse("a30b6cd8-2e20-423e-9d81-b48b42ef9f0b");
            var result = await _client.DeleteAsync("api/book/" + bookGuid);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Book_pagination_default_parameters()
        {
            await _client.LoginAsLibrarian();

            // reverse order to test ordering
            for (int i = 5; i > 0; i--)
            {
                BookDto bookToCreate = new BookDto { Title = "Title " + i, Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);
            }

            await _client.LogoutUser();

            var result = await _client.GetJsonAsync<PagingResultDto<BookDto>>("api/book/page");
            var books = result.Items ?? Array.Empty<BookDto>();
            Assert.Equal(5, books.Count);

            Assert.Equal(books.OrderBy(x => x.Title), books); // assert books are ordered by title

            Assert.Equal(0, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(5, result.TotalCount);
        }

        [Fact]
        public async Task Book_pagination_with_params()
        {
            await _client.LoginAsLibrarian();

            // reverse order to test ordering
            for (int i = 0; i < 30; i++)
            {
                BookDto bookToCreate = new BookDto { Title = $"Title {i:D3}", Author = "Author " + i, Description = "Descr " + i, Isbn13 = "ISBN 13" + i };
                await _client.PostJsonAsync<EntityId>("api/book", bookToCreate);
            }

            await _client.LogoutUser();

            int limit = 7;
            int page = 2;

            var result = await _client.GetJsonAsync<PagingResultDto<BookDto>>($"api/book/page?limit={limit}&page={page}");
            var books = result.Items ?? Array.Empty<BookDto>();
            Assert.Equal(limit, books.Count);

            var expectedTitles = Enumerable.Range(limit * page, limit).Select(i => $"Title {i:D3}");
            Assert.Equal(expectedTitles, books.Select(x => x.Title));

            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(5, result.TotalPages);
            Assert.Equal(30, result.TotalCount);
        }
    }
}
