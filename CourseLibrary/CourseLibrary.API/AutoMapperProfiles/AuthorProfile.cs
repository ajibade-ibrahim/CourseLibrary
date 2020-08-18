using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Utilities;
using CourseLibrary.Domain;

namespace CourseLibrary.API.AutoMapperProfiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(
                    dest => dest.Name,
                    options => options.MapFrom(source => $"{source.FirstName} {source.LastName}"))
                .ForMember(
                    dest => dest.Age,
                    options => options.MapFrom(source => $"{source.DateOfBirth.GetCurrentAge()}"));

            CreateMap<AuthorCreationDto, Author>();
        }
    }
}