using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Utilities
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IReadOnlyCollection<T> items, int currentPage, int pageSize, int totalItemCount)
        {
            if (items?.Count > totalItemCount)
            {
                throw new ArgumentException("Item count cannot be greater than the total number of items.");
            }

            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalItemCount;

            if (items != null)
            {
                AddRange(items);
            }
        }

        public int CurrentPage { get; set; }

        public bool HasNextPage
        {
            get => CurrentPage < Math.Ceiling(TotalCount / (double)PageSize);
        }

        public bool HasPreviousPage
        {
            get => CurrentPage > 1;
        }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}