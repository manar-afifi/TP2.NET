using AutoMapper;
using Gauniv.WebServer.Data;

namespace Gauniv.WebServer.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping entre Game et GameDto
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Name).ToList()));

            // Mapping entre User et UserDto
            CreateMap<User, UserDto>();

            // Mapping entre Category et CategoryDto
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Games, opt => opt.MapFrom(src => src.Games.Select(g => g.Name).ToList()));
        }
    }
}
