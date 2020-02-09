using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryWebsite.Books;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebsite
{
    public class SampleDataSeeder
    {
        private readonly LibraryContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SampleDataSeeder(LibraryContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async ValueTask SetupExampleData()
        {
            await SetupBooks();

            await SetupUsers();
        }

        private async Task SetupBooks()
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

        private async Task SetupUsers()
        {
            if (await _userManager.Users.AnyAsync())
                return;

            var admin = new ApplicationUser {UserName = "admin@sample.com", Email = "admin@sample.com"};
            await _userManager.CreateAsync(admin, "Abcdefgh!1");
            await _userManager.AddToRoleAsync(admin, Role.Admin);

            var librarian = new ApplicationUser {UserName = "librarian@sample.com", Email = "librarian@sample.com"};
            await _userManager.CreateAsync(librarian, "Abcdefgh!1");
            await _userManager.AddToRoleAsync(librarian, Role.Librarian);

            var reader = new ApplicationUser {UserName = "reader@sample.com", Email = "reader@sample.com"};
            await _userManager.CreateAsync(reader, "Abcdefgh!1");
            await _userManager.AddToRoleAsync(reader, Role.Reader);
        }
    }
}