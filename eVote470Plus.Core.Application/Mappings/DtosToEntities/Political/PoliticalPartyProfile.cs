using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Domain.Entities.Political;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.Political
{
    public class PoliticalPartyProfile : Profile
    {
        public PoliticalPartyProfile()
        {
            CreateMap<PoliticalParty, PoliticalPartyDto>();

            CreateMap<PoliticalPartyDto, PoliticalParty>()
                .ForMember(dest => dest.Candidates, opt => opt.Ignore())
                .ForMember(dest => dest.SentAlliances, opt => opt.Ignore())
                .ForMember(dest => dest.ReceivedAlliances, opt => opt.Ignore())
                .ForMember(dest => dest.PoliticalLeader, opt => opt.Ignore());
        }
    }
}
