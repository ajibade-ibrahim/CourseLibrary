using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.Domain;

namespace CourseLibrary.API.AutoMapperProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<CourseCreationDto, Course>();
        }
    }
}