using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.Domain;
using CourseLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorCollectionsController : ControllerBase
    {
        public AuthorCollectionsController(IMapper mapper, ICourseLibraryRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        private readonly IMapper _mapper;
        private readonly ICourseLibraryRepository _repository;

        [HttpGet("({keys})", Name = "GetAuthorsByKeys")]
        public ActionResult<IEnumerable<AuthorDto>> Get([FromRoute] string keys)
        {
            if (string.IsNullOrWhiteSpace(keys))
            {
                return BadRequest();
            }

            var authorIds = keys.Split(',');
            var authors = _repository.GetAuthorsByIds(authorIds);

            if (authors.Count() != authorIds.Length)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors));
        }

        [HttpPost]
        public IActionResult Post(IEnumerable<AuthorCreationDto> authorCreationDtos)
        {
            var authors = _mapper.Map<IEnumerable<Author>>(authorCreationDtos);
            _repository.AddAuthors(authors);
            _repository.Save();

            var authorIds = _mapper.Map<List<AuthorDto>>(authors).Select(author => author.Id);
            var keys = string.Join(',', authorIds);

            var result = CreatedAtRoute(
                "GetAuthorsByKeys",
                new
                {
                    keys
                },
                keys);

            return result;
        }
    }
}