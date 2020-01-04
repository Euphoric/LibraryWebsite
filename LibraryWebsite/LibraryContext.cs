using LibraryWebsite.Books;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Options;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.Options;

namespace LibraryWebsite
{
    public class LibraryContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public LibraryContext(
            DbContextOptions<LibraryContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            :base(options, operationalStoreOptions)
        {
        }

        public DbSet<Book> Books { get; set; }

        internal async ValueTask SetupExampleData()
        {
            if (Books.Any())
                return;

            BookCsvParser parser = new BookCsvParser();
            var parsedBooks = parser.Parse("SampleData/books-small1.csv");

            foreach (var parsedBook in parsedBooks)
            {
                var book = new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = parsedBook.Title,
                    Author = parsedBook.Authors,
                    Isbn13 = parsedBook.Isbn13
                };

                await Books.AddAsync(book);
            }

            await SaveChangesAsync();
        }
    }
}
