using AutoMapper;
using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Models.DTO.Auth;

namespace DMNRestaurant.Helper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserCreateDTO>().ReverseMap();
            CreateMap<User, UserUpdateDTO>().ReverseMap();

            //CreateMap<User, UserDTO>();
            //CreateMap<UserDTO, User>();

            //CreateMap<User, UserCreateDTO>();
            //CreateMap<UserCreateDTO, User>();

            //CreateMap<User, UserUpdateDTO>();
            //CreateMap<UserUpdateDTO, User>();
        }
    }
}
