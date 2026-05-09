using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Domain.Entities.Political;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.Political
{
    public class ElectionPositionProfile : Profile
    {
        public ElectionPositionProfile()
        {
            CreateMap<ElectionPosition, ElectionPositionDto>();

            CreateMap<ElectionPositionDto, ElectionPosition>()
                .ForMember(dest => dest.Election, opt => opt.Ignore())
                .ForMember(dest => dest.PoliticalPosition, opt => opt.Ignore());
        }
    }
}
