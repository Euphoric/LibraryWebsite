using System;

namespace LibraryWebsite.Books
{
    public class Book
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Isbn13 { get; set; }
    }
}
