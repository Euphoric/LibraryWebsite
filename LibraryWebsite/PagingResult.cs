using System.Collections.Generic;

namespace LibraryWebsite
{
    public class PagingResult<TItem>
    {
        public PagingResult(TItem[] items, int currentPage, int totalPages, int totalCount)
        {
            Items = items;
            CurrentPage = currentPage;
            TotalPages = totalPages;
            TotalCount = totalCount;
        }

        public TItem[] Items { get; }

        public int CurrentPage { get; }

        public int TotalPages { get; }

        public int TotalCount { get;  }
    }
}
