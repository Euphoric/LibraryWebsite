using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebsite
{
    public class PagingResult<TItem>
    {
        public PagingResult()
        {
        }

        public PagingResult(TItem[] items)
        {
            Items = items;
        }

        public TItem[] Items { get; set; }
    }
}
