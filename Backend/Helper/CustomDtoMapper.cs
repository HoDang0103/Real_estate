using AutoMapper;
using Backend.Models;
using Backend.Repository.UserService.Dtos;

namespace Backend.Mapper
{
    public class CustomDtoMapper : Profile
    {
        public CustomDtoMapper() 
        {
            //User Service
            CreateMap<ApplicationUser, LockUserDto>().ReverseMap();
            CreateMap<ApplicationUser, ChangePasswordDto>().ReverseMap();
            CreateMap<ApplicationUser, GetAllUserDto>().ReverseMap();
            CreateMap<ApplicationUser, GetDetailUserDto>();
        }
    }
}
