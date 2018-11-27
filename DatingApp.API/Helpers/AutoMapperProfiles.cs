using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    // auto mapper uses profiles to understand the relationships
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            // formember customizes configuration for individual member of the class
            // ForMember -> MapFrom tells that you want the value of one property "projected" onto the other property
            CreateMap<User, UserForListDTO>().
            ForMember(dest => dest.PhotoUrl, opt => {
                // to get the photo url displayed for the user dto as well
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                // CalculateAge is an extension written for Datetime in Extensions.cs
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });

            CreateMap<User, UserForDetailedDTO>().
            ForMember(dest => dest.PhotoUrl, opt => {
                // to get the photo url displayed for the user dto as well
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            }).ForMember(dest => dest.Age, opt => {
                // CalculateAge is an extension written for Datetime in Extensions.cs
                // ResolveUsing - Resolve destination member using a custom value resolver callback. 
                // Used instead of MapFrom when not simply redirecting a source member 
                // (MapFrom only allows redirection of properties, whereas ResolveUsing can be anything, like a function as below)
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            
            CreateMap<Photo, PhotosForDetailedDTO>();
        }
    }
}