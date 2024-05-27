using api.Dtos;
using AutoMapper;
using Dtos.Cart;
using Dtos.Category;
using Dtos.Orders;
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
            CreateMap<Product, UpdateProductDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Cart, CartDto>();


            CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

            CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        }
    }
}