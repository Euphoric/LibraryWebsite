using LibraryWebsite.Books;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebsite
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            :base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        internal void SetupExampleData()
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

                Books.Add(book);
            }

            SaveChanges();
        }
    }
}
