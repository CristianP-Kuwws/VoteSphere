using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.People;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.People
{
    public class CitizenRepository : GenericRepository<Citizen>, ICitizenRepository
    {
        public CitizenRepository(EVote470PlusContext context) : base(context)
        {

        }

        public async Task<bool> ChangeActiveStatusAsync(int citizenId, bool isActive)
        {
            try
            {
                var candidate = await GetByIdAsync(citizenId);

                if (candidate == null)
                {
                    return false;
                }

                candidate.IsActive = isActive;
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to activate Citizen: {nameof(Citizen)}", ex);
            }
        }

        public async Task<bool> HasVotedInElectionAsync(int citizenId, int electionId)
        {
            return await _context.Votes.AnyAsync(v => v.CitizenId == citizenId && v.ElectionId == electionId);
        }

        public async Task<Citizen?> GetByDocumentNumberAsync(string documentNumber)
        {
            return await _context.Citizens.FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber);
        }

    }
}

    