using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.ViewModels.Political.PoliticalPosition;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.Political
{
    public class PoliticalPositionViewModelMappingProfile : Profile
    {
        public PoliticalPositionViewModelMappingProfile()
        {
            CreateMap<PoliticalPositionViewModel, PoliticalPositionDto>().ReverseMap();

            CreateMap<SavePoliticalPositionViewModel, PoliticalPositionDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<PoliticalPositionDto, SavePoliticalPositionViewModel>();

        }
    }
}
