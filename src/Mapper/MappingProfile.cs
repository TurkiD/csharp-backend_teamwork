using api.Dtos;
using AutoMapper;
using Dtos.User.Profile;

namespace api.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserProfileDto>();
        }
    }
}