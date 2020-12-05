﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Euphoric.EventModel;
using LibraryWebsite.Books;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebsite
{
    [DomainEvent("sample-data-seeded")]
    public sealed record SampleDataSeeded : IDomainEventData
    {
        public static readonly string Key = "sample-data-seeded";

        public string GetAggregateKey()
        {
            return Key;
        }
    }

    public class SampleDataSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventStore _eventStore;

        public SampleDataSeeder(UserManager<ApplicationUser> userManager, IEventStore eventStore)
        {
            _userManager = userManager;
            _eventStore = eventStore;
        }

        public async ValueTask SetupExampleData()
        {
            await SetupBooks();

            await SetupUsers();
        }

        private async Task SetupBooks()
        {
            if (await _eventStore.GetAggregateEvents(SampleDataSeeded.Key).AnyAsync())
                return;

            BookCsvParser parser = new BookCsvParser();
            var parsedBooks = parser.Parse("SampleData/books-small1.csv");

            foreach (var parsedBook in parsedBooks)
            {
                var evnt = BookAggregate.New(parsedBook.Title!, parsedBook.Authors!, parsedBook.Isbn13!, "");
                await _eventStore.Store(evnt);
            }

            await _eventStore.Store(new SampleDataSeeded().AsNewAggregate());
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