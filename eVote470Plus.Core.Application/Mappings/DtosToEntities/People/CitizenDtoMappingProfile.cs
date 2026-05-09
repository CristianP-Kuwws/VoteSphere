using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Domain.Entities.People;

namespace eVote470Plus.Core.Application.Mappings.DtosToEntities.People
{
    public class CitizenDtoMappingProfile : Profile
    {
        public CitizenDtoMappingProfile()
        {

            CreateMap<Citizen, CitizenDto>();

            CreateMap<CitizenDto, Citizen>()
                .ForMember(dest => dest.Votes, opt => opt.Ignore());
        }
    }
}
