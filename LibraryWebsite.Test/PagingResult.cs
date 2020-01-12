using System.Collections.Generic;

namespace LibraryWebsite
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "DTO")]
    public class PagingResultDto<TItem>
    {
        public ICollection<TItem> Items { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
