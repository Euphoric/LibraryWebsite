using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebsite.Books
{
    public class BookDto
    {
        public EntityId Id { get; set; } = EntityId.Empty;

        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? Isbn13 { get; set; }

        public string? Description { get; set; }
    }
}
