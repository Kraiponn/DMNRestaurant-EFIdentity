using AutoMapper;
using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Models;
using DMNRestaurant.Models.DTO.Auth;
using DMNRestaurant.Models.DTO.Category;
using DMNRestaurant.Models.DTO.Product;

namespace DMNRestaurant.Helper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserCreateDTO>().ReverseMap();
            CreateMap<User, UserUpdateDTO>().ReverseMap();

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryCUDTO>().ReverseMap();

            CreateMap<Product, ProductCUDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();

            //CreateMap<User, UserDTO>();
            //CreateMap<UserDTO, User>();

            //CreateMap<User, UserCreateDTO>();
            //CreateMap<UserCreateDTO, User>();

            //CreateMap<User, UserUpdateDTO>();
            //CreateMap<UserUpdateDTO, User>();
        }
    }
}
