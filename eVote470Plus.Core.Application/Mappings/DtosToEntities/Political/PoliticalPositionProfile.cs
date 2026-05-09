using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Domain.Entities.Political;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.Political
{
    public class PoliticalPositionProfile : Profile
    {
        public PoliticalPositionProfile()
        {
            CreateMap<PoliticalPosition, PoliticalPositionDto>();

            CreateMap<PoliticalPositionDto, PoliticalPosition>();
             
        }
    }
}
