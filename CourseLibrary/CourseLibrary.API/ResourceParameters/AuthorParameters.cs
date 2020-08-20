using CourseLibrary.Services.ResourceParameterContracts;

namespace CourseLibrary.API.ResourceParameters
{
    public class AuthorParameters : IAuthorParameters
    {
        private int _maximumPageSize;

        public string MainCategory { get; set; }

        public int MaximumPageSize
        {
            get => _maximumPageSize;
            set
            {
                _maximumPageSize = value;

                if (_maximumPageSize > 0 && PageSize > _maximumPageSize)
                {
                    PageSize = _maximumPageSize;
                }
            }
        }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchQuery { get; set; }
    }
}