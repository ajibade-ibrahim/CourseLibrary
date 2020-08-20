using System;
using System.Collections.Generic;
using CourseLibrary.Domain;
using CourseLibrary.Services.ResourceParameterContracts;

namespace CourseLibrary.Services
{
    public interface ICourseLibraryRepository
    {
        void AddAuthor(Author author);
        void AddAuthors(IEnumerable<Author> authors);
        void AddCourse(Guid authorId, Course course);
        bool AuthorExists(Guid authorId);
        void DeleteAuthor(Author author);
        void DeleteCourse(Course course);
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors();
        IEnumerable<Author> GetAuthors(IAuthorParameters authorParameters, out int totalCount);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        Course GetCourse(Guid authorId, Guid courseId);
        IEnumerable<Course> GetCourses(Guid authorId);
        bool Save();
        void UpdateAuthor(Author author);
        void UpdateCourse(Course course);
        IEnumerable<Author> GetAuthorsByIds(IEnumerable<string> authorIds);
    }
}