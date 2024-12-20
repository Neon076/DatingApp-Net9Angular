using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {   

        // CreateMap<>() <source , destination>
        CreateMap<AppUser, MembersDto>()
        .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
        .ForMember(d => d.PhotoUrl, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto , AppUser>();
        CreateMap<ResigterDto , AppUser>();
        CreateMap<string , DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
    }
}