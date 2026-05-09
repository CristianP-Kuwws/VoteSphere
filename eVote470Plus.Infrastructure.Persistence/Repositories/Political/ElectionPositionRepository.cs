using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Political;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.Political
{
    public class ElectionPositionRepository : GenericRepository<ElectionPosition>, IElectionPositionRepository
    {
        public ElectionPositionRepository(EVote470PlusContext context) : base(context)
        {
        }
    }
}
