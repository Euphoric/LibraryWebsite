using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebsite.Books
{
    public class BookCsvParser
    {
        public class ParsedBook
        {
            [Name("book_id")]
            public int Id { get; set; }

            [Name("original_title")]
            public string Title { get; set; }

            [Name("authors")]
            public string Authors { get; set; }

            [Name("isbn13")]
            [TypeConverter(typeof(Isbn13Converter))]
            public string Isbn13 { get; set; }
        }

#pragma warning disable CA1812 // Internal class that is never instantiated
        private class Isbn13Converter : DefaultTypeConverter
#pragma warning restore CA1812 // Internal class that is never instantiated
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;

                var val = double.Parse(text, CultureInfo.InvariantCulture);
                return val.ToString("G13", CultureInfo.InvariantCulture);
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                throw new NotSupportedException();
            }
        }

        public IEnumerable<ParsedBook> Parse(string booksFilename)
        {
            if (booksFilename == null)
                throw new ArgumentNullException(nameof(booksFilename));

            using (var reader = new StreamReader(booksFilename))
            {
                if (reader.Peek() == -1)
                {
                    throw new Exception("File is empty.");
                }
                
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.Delimiter = ",";
                    return csv.GetRecords<ParsedBook>().ToArray();
                }
            }
        }
    }
}
