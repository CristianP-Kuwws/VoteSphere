using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Interfaces.Relations;
using eVote470Plus.Core.Application.Services.People;
using eVote470Plus.Core.Application.Services.Political;
using eVote470Plus.Core.Application.Services.Relations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace eVote470Plus.Core.Application.IOC
{
    public static class ServicesRegistration
    {
        public static void AddApplicationLayerIOC(this IServiceCollection services)
        {
            // Mapper Configuration
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());


            // Services IOC
            services.AddScoped<ICandidateService, CandidateService>();
            services.AddScoped<ICitizenService, CitizenService>();
            services.AddScoped<IElectionService, ElectionService>();
            services.AddScoped<IPoliticalPartyService, PoliticalPartyService>();
            services.AddScoped<IPoliticalPositionService, PoliticalPositionService>();
            services.AddScoped<IPoliticalAllianceService, PoliticalAllianceService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IPoliticalLeaderService, PoliticalLeaderService>();
            services.AddScoped<ICandidatePositionService, CandidatePositionService>();

        }
    }
}
