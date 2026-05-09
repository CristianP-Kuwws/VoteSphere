using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Domain.Entities.People;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.People
{
    public class CandidateDtoMappingProfile : Profile
    {
        public CandidateDtoMappingProfile()
        {
            CreateMap<Candidate, CandidateDto>();

            CreateMap<CandidateDto, Candidate>()
                .ForMember(dest => dest.PoliticalParty, opt => opt.Ignore())
                .ForMember(dest => dest.CandidatePositions, opt => opt.Ignore())
                .ForMember(dest => dest.Votes, opt => opt.Ignore());
        }
    }
}
