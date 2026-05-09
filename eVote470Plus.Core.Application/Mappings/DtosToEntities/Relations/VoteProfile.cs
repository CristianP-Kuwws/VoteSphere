using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Domain.Entities.Relations;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.Relations
{
    public class VoteProfile : Profile
    {
        public VoteProfile()
        {
            CreateMap<Vote, VoteDto>();

            CreateMap<VoteDto, Vote>()
                .ForMember(dest => dest.Citizen, opt => opt.Ignore())
                .ForMember(dest => dest.Candidate, opt => opt.Ignore())
                .ForMember(dest => dest.Election, opt => opt.Ignore())
                .ForMember(dest => dest.PoliticalPosition, opt => opt.Ignore());
        }
    }
}
