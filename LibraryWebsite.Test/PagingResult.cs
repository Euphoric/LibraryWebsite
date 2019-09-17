namespace LibraryWebsite
{
    public class PagingResult<TItem>
    {
        public TItem[] Items { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
