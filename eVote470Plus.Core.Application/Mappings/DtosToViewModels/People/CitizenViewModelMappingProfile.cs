using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.ViewModels.People.Citizen;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.People
{
    public class CitizenViewModelMappingProfile : Profile
    {
        public CitizenViewModelMappingProfile()
        {
            CreateMap<CitizenViewmodel, CitizenDto>().ReverseMap();

            CreateMap<SaveCitizenViewModel, CitizenDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<CitizenDto, SaveCitizenViewModel>();
        }
    }
}
