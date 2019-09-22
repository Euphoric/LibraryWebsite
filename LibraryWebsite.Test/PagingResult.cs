namespace LibraryWebsite
{
    public class PagingResultDto<TItem>
    {
        public TItem[] Items { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
