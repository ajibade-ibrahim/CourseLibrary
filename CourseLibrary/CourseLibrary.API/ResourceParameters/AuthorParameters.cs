using CourseLibrary.Services.ResourceParameterContracts;

namespace CourseLibrary.API.ResourceParameters
{
    public class AuthorParameters : IAuthorParameters
    {
        public string MainCategory { get; set; }
        public string SearchQuery { get; set; }
    }
}