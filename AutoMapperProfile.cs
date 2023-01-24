using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos.Cart;

namespace WebApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap(typeof(ServiceResponse<>), (typeof(ServiceResponseDto<>)));
            CreateMap<Kit, Variant>();
            CreateMap<Kit, KitDto>()
                .ForMember(kitDto => kitDto.Id, kit => kit.MapFrom(src => src.KitId));
            CreateMap<Variant, VariantDto>()
                .ForMember(VariantDto => VariantDto.Id, kit => kit.MapFrom(src => src.VariantId));
        }
    }
}