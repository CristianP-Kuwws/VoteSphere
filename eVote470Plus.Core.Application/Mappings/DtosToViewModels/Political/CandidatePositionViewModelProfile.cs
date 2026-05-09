using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.ViewModels.Political.CandidatePosition;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.Political
{
    public class CandidatePositionViewModelProfile : Profile
    {
        public CandidatePositionViewModelProfile()
        {
            CreateMap<CandidatePositionViewModel, CandidatePositionDto>();

            CreateMap<CandidatePositionDto, CandidatePositionViewModel>()
                .ForMember(dest => dest.CandidateName, opt => opt.Ignore())
                .ForMember(dest => dest.PoliticalPositionName, opt => opt.Ignore());

            CreateMap<SaveCandidatePositionViewModel, CandidatePositionDto>().ReverseMap();
        }
    }
}
