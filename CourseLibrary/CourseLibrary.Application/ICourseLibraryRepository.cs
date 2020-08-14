using System;
using System.Collections.Generic;
using CourseLibrary.Domain;

namespace CourseLibrary.Services
{
    public interface ICourseLibraryRepository
    {
        void AddAuthor(Author author);
        void AddCourse(Guid authorId, Course course);
        bool AuthorExists(Guid authorId);
        void DeleteAuthor(Author author);
        void DeleteCourse(Course course);
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors();
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        Course GetCourse(Guid authorId, Guid courseId);
        IEnumerable<Course> GetCourses(Guid authorId);
        bool Save();
        void UpdateAuthor(Author author);
        void UpdateCourse(Course course);
    }
}