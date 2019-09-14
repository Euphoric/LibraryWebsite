using System.Collections.Generic;

namespace LibraryWebsite
{
    public class PagingResult<TItem>
    {
        public PagingResult(TItem[] items, int currentPage, int totalPages)
        {
            Items = items;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public TItem[] Items { get; }

        public int CurrentPage { get; }

        public int TotalPages { get; }
    }
}
