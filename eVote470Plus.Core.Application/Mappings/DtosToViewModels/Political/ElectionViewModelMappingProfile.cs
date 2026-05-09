using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.ViewModels.People.Candidate;
using eVote470Plus.Core.Application.ViewModels.Political.Election;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.Political
{
    public class ElectionViewModelMappingProfile : Profile
    {
        public ElectionViewModelMappingProfile()
        {
            CreateMap<ElectionViewModel, ElectionDto>();

            CreateMap<ElectionDto, ElectionViewModel>();

            CreateMap<SaveElectionViewModel, ElectionDto>()
                .ForMember(dest => dest.FinishedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<ElectionDto, SaveElectionViewModel>();

            // Results
            CreateMap<ElectionResultDto, ElectionResultViewModel>();
            CreateMap<CandidateResultDto, CandidateResultViewModel>();
        }
    }
}
