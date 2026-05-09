using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Application.ViewModels.Relations.Vote;

namespace eVote470Plus.Core.Application.Mappings.DtosToViewModels.Relations
{
    public class VoteViewModelMappingProfile : Profile
    {
        public VoteViewModelMappingProfile()
        {
            CreateMap<VoteViewModel, VoteDto>().ReverseMap();

            CreateMap<SaveVoteViewModel, VoteDto>();
        }
    }
}
