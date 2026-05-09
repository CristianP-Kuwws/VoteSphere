using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Application.ViewModels.Relations.PoliticalAlliance;
using eVote470Plus.Core.Domain.Common.Enum;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.Relations
{
    public class PoliticalAllianceViewModelMappingProfile : Profile
    {
        public PoliticalAllianceViewModelMappingProfile()
        {
            CreateMap<PoliticalAllianceViewModel, PoliticalAllianceDto>();

            CreateMap<PoliticalAllianceDto, PoliticalAllianceViewModel>();

            CreateMap<SavePoliticalAllianceViewModel, PoliticalAllianceDto>()
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => AllianceStatus.Pending))
                    .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.ResponseDate, opt => opt.Ignore()); 

            CreateMap<PoliticalAllianceDto, SavePoliticalAllianceViewModel>();
        }
    }
}
