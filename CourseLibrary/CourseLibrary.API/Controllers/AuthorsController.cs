using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _repository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [HttpHead]
        public IActionResult GetAuthors([FromQuery] AuthorParameters parameters)
        {
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(_repository.GetAuthors(parameters)));
        }

        [HttpGet("api/authors/{id}")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthor(Guid id)
        {
            var author = _repository.GetAuthor(id);

            return author == null
                ? (ActionResult<IEnumerable<AuthorDto>>)NotFound()
                : Ok(_mapper.Map<AuthorDto>(author));
        }
    }
}