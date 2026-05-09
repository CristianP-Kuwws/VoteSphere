using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.ViewModels.People.Candidate;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.People
{
    public class CandidateViewModelMappingProfile : Profile
    {
        public CandidateViewModelMappingProfile()
        {
            CreateMap<CandidateViewModel, CandidateDto>()
                .ForMember(dest => dest.PoliticalPartyId, opt => opt.Ignore());

            CreateMap<CandidateDto, CandidateViewModel>()
                .ForMember(dest => dest.PoliticalPartyPosition, opt => opt.Ignore());

            CreateMap<SaveCandidateViewModel, CandidateDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.PoliticalPartyId, opt => opt.Ignore());     

            CreateMap<CandidateDto, SaveCandidateViewModel>()
                .ForMember(dest => dest.CandidatePhoto, opt => opt.Ignore());


        }
    }
}
