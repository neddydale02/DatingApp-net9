using AutoMapper;
using API.Entities;
using API.DTOs;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
            .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
            .ForMember(d => d.PhotoUrl, o =>
             o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url)); //d = destinazione, o = opzioni, s = sorgente, x = elemento
            CreateMap<Photo, PhotoDto>();
        }
    }
}