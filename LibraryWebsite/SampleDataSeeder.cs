using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryWebsite.Books;

namespace LibraryWebsite
{
    public class SampleDataSeeder
    {
        private readonly LibraryContext _context;

        public SampleDataSeeder(LibraryContext context)
        {
            _context = context;
        }

        public async ValueTask SetupExampleData()
        {
            if (_context.Books.Any())
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

                await _context.Books.AddAsync(book);
            }

            await _context.SaveChangesAsync();
        }
    }
}