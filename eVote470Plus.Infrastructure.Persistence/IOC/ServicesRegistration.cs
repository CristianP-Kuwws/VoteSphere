using eVote470Plus.Core.Domain.Interfaces.People;
using eVote470Plus.Core.Domain.Interfaces.Political;
using eVote470Plus.Core.Domain.Interfaces.Relations;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.People;
using eVote470Plus.Infrastructure.Persistence.Repositories.Political;
using eVote470Plus.Infrastructure.Persistence.Repositories.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace eVote470Plus.Infrastructure.Persistence.IOC
{
    public static class ServicesRegistration
    {
        public static void AddPersistenceLayerIOC(this IServiceCollection services, IConfiguration config)
        {

            #region Contexts

            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<EVote470PlusContext>(options =>
                    options.UseInMemoryDatabase("eVote470PlusDb"));

                return;
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                services.AddDbContext<EVote470PlusContext>(options =>
                    options.UseMySql
                    (
                        connectionString,
                        ServerVersion.AutoDetect(connectionString),
                        mysqlOptions => mysqlOptions.MigrationsAssembly(typeof(EVote470PlusContext).Assembly.FullName)
                    ),
                    ServiceLifetime.Transient   
                );
            }


            #endregion

            // Mapper Configuration
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            // Repositories IOC
            services.AddTransient<ICandidateRepository, CandidateRepository>();
            services.AddTransient<ICitizenRepository, CitizenRepository>();
            services.AddTransient<IPoliticalLeaderRepository, PoliticalLeaderRepository>();
            services.AddTransient<ICandidatePositionRepository, CandidatePositionRepository>();
            services.AddTransient<IElectionPositionRepository, ElectionPositionRepository>();
            services.AddTransient<IElectionRepository, ElectionRepository>();
            services.AddTransient<IPoliticalPartyRepository, PoliticalPartyRepository>();
            services.AddTransient<IPoliticalPositionRepository, PoliticalPositionRepository>();
            services.AddTransient<IPoliticalAllianceRepository, PoliticalAllianceRepository>();
            services.AddTransient<IVoteRepository, VoteRepository>();

        }
    }
}
