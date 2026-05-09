using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Domain.Entities.People;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.People
{
    public class PoliticalLeaderProfile : Profile
    {
        public PoliticalLeaderProfile()
        {
            CreateMap<PoliticalLeader, PoliticalLeaderDto>();

            CreateMap<PoliticalLeaderDto, PoliticalLeader>()
                .ForMember(dest => dest.PoliticalParty, opt => opt.Ignore());
        }
    }
}
