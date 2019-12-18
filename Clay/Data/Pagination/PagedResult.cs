using System;
using System.Collections.Generic;

namespace Clay.Data.Pagination
{
    [Serializable]
    public class PagedResult<T> : PagedResultBase
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}