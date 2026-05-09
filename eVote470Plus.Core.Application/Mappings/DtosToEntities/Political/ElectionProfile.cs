using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Domain.Entities.Political;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.Political
{
    public class ElectionProfile : Profile
    {
        public ElectionProfile()
        {
            CreateMap<Election, ElectionDto>();

            CreateMap<ElectionDto, Election>()
                .ForMember(dest => dest.ElectionPositions, opt => opt.Ignore())
                .ForMember(dest => dest.Votes, opt => opt.Ignore());
        }
    }
}
