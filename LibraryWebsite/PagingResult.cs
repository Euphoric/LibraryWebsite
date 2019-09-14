using System.Collections.Generic;

namespace LibraryWebsite
{
    public class PagingResult<TItem>
    {
        public PagingResult()
        {
        }

        public PagingResult(TItem[] items, int currentPage, int totalPages)
        {
            Items = items;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public TItem[] Items { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
