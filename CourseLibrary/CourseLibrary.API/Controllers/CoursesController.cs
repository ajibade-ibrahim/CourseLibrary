using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Models;
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

        // GET: api/Course
        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> Get(Guid authorId)
        {
            var author = _repository.GetAuthor(authorId);

            return author == null
                ? (ActionResult<IEnumerable<CourseDto>>)NotFound()
                : Ok(_mapper.Map<IEnumerable<CourseDto>>(_repository.GetCourses(authorId)));
        }

        [HttpGet]
        [Route("{courseId}")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _repository.GetCourse(authorId, courseId);
            return course == null ? (ActionResult<CourseDto>)NotFound() : _mapper.Map<CourseDto>(course);
        }
    }
}
