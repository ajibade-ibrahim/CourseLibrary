using System;
using System.Collections.Generic;
using System.Linq;
using CourseLibrary.Domain;
using CourseLibrary.Services;
using CourseLibrary.Services.ResourceParameterContracts;

namespace CourseLibrary.Persistence.EFCore
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        public CourseLibraryRepository(CourseLibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private readonly CourseLibraryContext _context;

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public void AddAuthors(IEnumerable<Author> authors)
        {
            if (authors == null)
            {
                throw new ArgumentNullException(nameof(authors));
            }

            // the repository fills the id (instead of using identity columns)
            var authorsList = authors.ToList();
            authorsList.ForEach(
                author =>
                {
                    author.Id = Guid.NewGuid();

                    if (author.Courses?.Any() == true)
                    {
                        foreach (var course in author.Courses)
                        {
                            course.Id = Guid.NewGuid();
                        }
                    }
                });

            _context.Authors.AddRange(authorsList);
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(author => author.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }

        public Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(author => author.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.ToList();
        }

        public IEnumerable<Author> GetAuthors(IAuthorParameters authorParameters, out int totalCount)
        {
            if (authorParameters == null)
            {
                throw new ArgumentNullException(
                    nameof(authorParameters),
                    "Filtering parameters should be passed to avoid data overload.");
            }

            if (authorParameters.PageNumber < 1 || authorParameters.PageSize < 1)
            {
                throw new ArgumentException("Invalid page information passed");
            }

            var authors = _context.Authors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(authorParameters.MainCategory))
            {
                var mainCategory = authorParameters.MainCategory.Trim();
                authors = authors.Where(author => author.MainCategory.Equals(mainCategory));
            }

            if (!string.IsNullOrWhiteSpace(authorParameters.SearchQuery))
            {
                var searchQuery = authorParameters.SearchQuery.Trim();
                authors = authors.Where(
                    author => author.FirstName.Contains(searchQuery)
                        || author.LastName.Contains(searchQuery)
                        || author.MainCategory.Contains(searchQuery));
            }

            totalCount = authors.Count();
            var skipCount = (authorParameters.PageNumber - 1) * authorParameters.PageSize;
            authors = authors.Skip(skipCount).Take(authorParameters.PageSize);
            return authors.ToList();
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToList();
        }

        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _context.Courses.FirstOrDefault(course => course.AuthorId == authorId && course.Id == courseId);
        }

        public IEnumerable<Course> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Courses.Where(course => course.AuthorId == authorId).OrderBy(c => c.Title).ToList();
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
        }

        public IEnumerable<Author> GetAuthorsByIds(IEnumerable<string> authorIds)
        {
            var idsList = authorIds?.ToList() ?? new List<string>();
            return idsList.Select(authorId => _context.Authors.Find(new Guid(authorId))).ToList();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}