using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Utilities;
using CourseLibrary.Domain;
using CourseLibrary.Services;
using CourseLibrary.Services.ResourceParameterContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private const string GetMethod = "GET";
        private const string DeleteAuthor = "DeleteAuthor";

        public AuthorsController(ICourseLibraryRepository repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ICourseLibraryRepository _repository;

        [HttpGet("{id}", Name = "GetAuthor")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthor(Guid id)
        {
            var author = _repository.GetAuthor(id);

            return author == null
                ? (ActionResult<IEnumerable<AuthorDto>>)NotFound()
                : Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        public IActionResult GetAuthors([FromQuery] AuthorParameters parameters)
        {
            if (parameters.PageNumber < 1)
            {
                ModelState.AddModelError("PageNumber", "PageNumber should not be less than 1.");
                return BadRequest();
            }

            parameters.MaximumPageSize =
                _configuration.GetSection("CollectionBoundaries").GetValue<int>("MaximumAuthorPageSize");

            var authors = _mapper.Map<IEnumerable<AuthorDto>>(_repository.GetAuthors(parameters, out var totalCount));
            var pagedList = new PagedList<AuthorDto>(
                authors.ToList(),
                parameters.PageNumber,
                parameters.PageSize,
                totalCount);

            Response.Headers.Add("X-Pagination", GetPaginationData(pagedList, parameters, totalCount));
            return Ok(pagedList);
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

        [HttpDelete("{authorId}", Name = DeleteAuthor)]
        public IActionResult Delete(Guid authorId)
        {
            var author = _repository.GetAuthor(authorId);

            if (author == null)
            {
                return NotFound();
            }

            _repository.DeleteAuthor(author);
            _repository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, HEAD, OPPTIONS");
            return Ok();
        }

        private string GetPaginationData(PagedList<AuthorDto> pagedList, IAuthorParameters parameters, int totalCount)
        {
            var totalPages = Math.Ceiling(totalCount / (double)parameters.PageSize);

            var previousPageLink =
                parameters.PageNumber == 1 ? null : GetPageLink(parameters, parameters.PageNumber - 1);

            var nextPageLink = totalPages > parameters.PageNumber
                ? GetPageLink(parameters, parameters.PageNumber + 1)
                : null;

            var paginationMetadata = new
            {
                pageSize = pagedList.PageSize,
                pageNumber = pagedList.CurrentPage,
                totalPages,
                totalCount,
                previousPageLink,
                nextPageLink
            };

            return JsonConvert.SerializeObject(paginationMetadata);
        }

        private string GetPageLink(IAuthorParameters parameters, int pageNumber)
        {
            return Url.Link(
                "GetAuthors",
                new
                {
                    pageNumber,
                    fields = parameters.Fields,
                    pageSize = parameters.PageSize,
                    mainCategory = parameters.MainCategory,
                    searchQuery = parameters.SearchQuery
                });
        }

        private IEnumerable<LinkDto> CreateAuthorLinks(Guid authorId, string fields)
        {
            var links = new List<LinkDto>();
            var authorIdObject = new
            {
                authorId
            };

            var authorIdWithFields = new
            {
                authorId,
                fields
            };

            var selfLink = string.IsNullOrWhiteSpace(fields)
                ? Url.Link("GetAuthor", authorIdObject)
                : Url.Link("GetAuthor", authorIdWithFields);

            links.Add(new LinkDto(selfLink, "self", GetMethod));
            links.Add(new LinkDto(Url.Link(DeleteAuthor, authorIdObject), "delete_author", "DELETE"));

            links.Add(
                new LinkDto(Url.Link("CreateCourseForAuthor", authorIdObject), "create_course_for_author", "POST"));
            links.Add(new LinkDto(Url.Link("GetCoursesForAuthor", authorIdObject), "courses", GetMethod));

            return links;
        }
    }
}