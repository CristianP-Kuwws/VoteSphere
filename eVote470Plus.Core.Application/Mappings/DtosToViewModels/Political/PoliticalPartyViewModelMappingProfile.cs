using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.ViewModels.Political.PoliticalParty;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.Political
{
    public class PoliticalPartyViewModelMappingProfile : Profile
    {
        public PoliticalPartyViewModelMappingProfile()
        {
            CreateMap<PoliticalPartyViewModel, PoliticalPartyDto>();

            CreateMap<PoliticalPartyDto, PoliticalPartyViewModel>();

            CreateMap<SavePoliticalPartyViewModel, PoliticalPartyDto>()
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore());

            CreateMap<PoliticalPartyDto, SavePoliticalPartyViewModel>()
                .ForMember(dest => dest.LogoImg, opt => opt.Ignore());
        }
    }
}
