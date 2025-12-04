using AutoMapper;
using MagicVillaApi.Models;
using MagicVillaApi.Models.DTO;

namespace MagicVillaApi
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<VillaList, VillaDto>().ReverseMap();
            CreateMap<VillaList, VillaCreateDto>().ReverseMap();
            CreateMap<VillaList, VillaUpdateDto>().ReverseMap();


            CreateMap<VillaNumber, VillaNumberDto>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreateDto>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDto>().ReverseMap();
            
            CreateMap<VillaNumberv2, VilaNumberDtov2>().ReverseMap();
            CreateMap<VillaNumberv2, VillaNumberCreateDtov2>().ReverseMap();
            CreateMap<VillaNumberv2, VillaNumberUpdateDtov2>().ReverseMap();

        }
    }
}
