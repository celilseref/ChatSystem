using System.Linq;
using AutoMapper;
using ServerApp.DTO;
using ServerApp.models;
using ServerApp.Models;

namespace ServerApp.Helpers
{
    public class MapperProfiles:Profile 
    {
        public MapperProfiles()
        {
           
              CreateMap<UserForUpdateDTO, User>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
    .ForMember(dest => dest.userImg, opt => opt.MapFrom(src => src.userImg))
    .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio));
    
          
        }
    }
}