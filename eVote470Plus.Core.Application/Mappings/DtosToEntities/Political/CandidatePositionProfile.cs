using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Domain.Entities.Political;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.Political
{
    public class CandidatePositionProfile : Profile
    {
        public CandidatePositionProfile()
        {
            CreateMap<CandidatePosition, CandidatePositionDto>();

            CreateMap<CandidatePositionDto, CandidatePosition>()
                .ForMember(dest => dest.Candidate, opt => opt.Ignore())
                .ForMember(dest => dest.PoliticalPosition, opt => opt.Ignore())
                .ForMember(dest => dest.PoliticalParty, opt => opt.Ignore());
                
        }
    }
}
