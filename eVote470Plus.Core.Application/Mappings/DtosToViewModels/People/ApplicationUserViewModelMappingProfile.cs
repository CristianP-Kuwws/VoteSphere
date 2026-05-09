using AutoMapper;
using eVote470Plus.Core.Application.Dtos.ApplicationUser;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.ViewModels.People.ApplicationUser;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.People
{
    public class ApplicationUserViewModelMappingProfile : Profile
    {

        public ApplicationUserViewModelMappingProfile()
        {
           
            CreateMap<ApplicationUserDto, ApplicationUserViewModel>()
                .ForMember(dest => dest.Role, opt => opt.Ignore()); 

            CreateMap<CreateApplicationUserViewModel, CreateApplicationUserDto>();

            CreateMap<ApplicationUserDto, EditApplicationUserViewModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore()); 

            CreateMap<EditApplicationUserViewModel, EditApplicationUserDto>();
        }
    }
}
