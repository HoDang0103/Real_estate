using AutoMapper;
using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Backend.Repository.Transaction.Dtos;
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

            //Story 
            //CreateMap<CreateStoryDto, Story>().ReverseMap();
            CreateMap<Story, StoryDto>().ReverseMap();
            CreateMap<Image, ImageDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<Catalog, CatalogDto>().ReverseMap();
            CreateMap<Package, PackageDto>().ReverseMap();

            //TopUp
            CreateMap<TopUp, TopUpDto>().ReverseMap();
            CreateMap<ApplicationUser, UserForTopUp>().ReverseMap();
            CreateMap<TopUp, MyTopUpDto>().ReverseMap();
        }
    }
}
