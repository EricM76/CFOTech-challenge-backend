using AutoMapper;
using ChallengeCFOTech.Models;
using ChallengeCFOTech.Models.Dtos;

namespace ChallengeCFOTech.Mappers
{
    public class IndexMapper : Profile
    {
        public IndexMapper() 
        {
            CreateMap<User, UserDto>().ReverseMap();
        
        }
    }
}
