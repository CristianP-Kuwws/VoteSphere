using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Domain.Entities.Relations;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.Relations
{
    public class PoliticallAllianceProfile : Profile
    {
        public PoliticallAllianceProfile()
        {

            CreateMap<PoliticalAlliance, PoliticalAllianceDto>();

            CreateMap<PoliticalAllianceDto, PoliticalAlliance>()
                .ForMember(dest => dest.SenderParty, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiverParty, opt => opt.Ignore());

        }
    }
}
