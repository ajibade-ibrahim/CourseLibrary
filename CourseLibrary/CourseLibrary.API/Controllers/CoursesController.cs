using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.Domain;
using CourseLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors/{authorId}/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _repository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetCoursesForAuthor")]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            var author = _repository.GetAuthor(authorId);

            return author == null
                ? (ActionResult<IEnumerable<CourseDto>>)NotFound()
                : Ok(_mapper.Map<IEnumerable<CourseDto>>(_repository.GetCourses(authorId)));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _repository.GetCourse(authorId, courseId);
            return course == null ? (ActionResult<CourseDto>)NotFound() : _mapper.Map<CourseDto>(course);
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid authorId, [FromBody] CourseCreationDto courseCreationDto)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _mapper.Map<Course>(courseCreationDto);
            _repository.AddCourse(authorId, course);
            var isSaved = _repository.Save();

            if (!isSaved)
            {
                throw new InvalidOperationException("Error occurred: Unable to create course");
            }

            var courseDto = _mapper.Map<CourseDto>(course);

            return CreatedAtAction(
                "GetCourseForAuthor",
                new
                {
                    authorId = authorId,
                    courseId = courseDto.Id
                },
                courseDto);
        }
    }
}
