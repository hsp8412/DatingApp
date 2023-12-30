using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
        .ForMember(dest => dest.PhotoUrl, opt=>opt.MapFrom(src => src.Photos.Where(x=>x.IsMain).Select(x=>x.Url).FirstOrDefault()))
        .ForMember(dest => dest.Age, opt=> opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>();
    }
}
