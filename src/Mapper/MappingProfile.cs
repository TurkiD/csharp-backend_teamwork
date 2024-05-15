using api.Dtos;
using AutoMapper;
using Dtos.OrderDto;
using Dtos.Product;
using Dtos.User.Profile;
using EntityFramework;

namespace api.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserProfileDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<Product, ProductOrderDto>();
            CreateMap<Order, OrderDto>();
        }
    }
}