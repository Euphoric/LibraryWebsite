using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace LibraryWebsite.Books
{
    public class BookCsvParserTest
    {
        [Fact]
        public void Parse_empty_path_is_error()
        {
            var parser = new BookCsvParser();
            Assert.Throws<ArgumentException>(() => parser.Parse(""));
        }

        [Fact]
        public void Parse_nonexistent_file_is_error()
        {
            var parser = new BookCsvParser();
            Assert.Throws<FileNotFoundException>(() => parser.Parse("no_file.csv"));
        }

        [Fact]
        public void Empty_file_is_error()
        {
            var parser = new BookCsvParser();
            Assert.Throws<Exception>(() => parser.Parse("Books/TestData/empty_file.csv"));
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(2, 21)]
        public void Parses_file_lines(int datasetId, int expectedCount)
        {
            var parser = new BookCsvParser();
            var books = parser.Parse($"Books/TestData/books-small{datasetId}.csv");
            Assert.NotNull(books);
            Assert.Equal(expectedCount, books.Count());

            var firstBook = books.First();

            BookCsvParser.ParsedBook expectedFirstBook = 
                datasetId == 1 ?
                new BookCsvParser.ParsedBook() { Id = 1, Title = "The Hunger Games", Authors = "Suzanne Collins", Isbn13 = "9780439023480" } :
                new BookCsvParser.ParsedBook() { Id = 110, Title = "A Clash of Kings", Authors = "George R.R. Martin", Isbn13 = "9780553381700" }
                ;

            Assert.Equal(expectedFirstBook.Id, firstBook.Id);
            Assert.Equal(expectedFirstBook.Title, firstBook.Title);
            Assert.Equal(expectedFirstBook.Authors, firstBook.Authors);
            Assert.Equal(expectedFirstBook.Isbn13, firstBook.Isbn13);
        }
    }
}
