using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.Domain;
using CourseLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        public AuthorsController(ICourseLibraryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        private readonly IMapper _mapper;
        private readonly ICourseLibraryRepository _repository;

        [HttpGet("{id}", Name = "GetAuthor")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthor(Guid id)
        {
            var author = _repository.GetAuthor(id);

            return author == null
                ? (ActionResult<IEnumerable<AuthorDto>>)NotFound()
                : Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpGet]
        [HttpHead]
        public IActionResult GetAuthors([FromQuery] AuthorParameters parameters)
        {
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(_repository.GetAuthors(parameters)));
        }

        [HttpPost]
        public ActionResult<AuthorDto> Post([FromBody] AuthorCreationDto authorCreationDto)
        {
            var author = _mapper.Map<Author>(authorCreationDto);
            _repository.AddAuthor(author);

            if (!_repository.Save())
            {
                throw new InvalidOperationException("Error occurred: Unable to create Author");
            }

            var authorDto = _mapper.Map<AuthorDto>(author);
            return CreatedAtAction(
                "GetAuthor",
                new
                {
                    id = authorDto.Id
                },
                authorDto);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, HEAD, OPPTIONS");
            return Ok();
        }
    }
}