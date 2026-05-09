using eVote470Plus.Core.Domain.Entities.Relations;
using eVote470Plus.Core.Domain.Interfaces.Relations;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.Relations
{
    public class VoteRepository : GenericRepository<Vote>, IVoteRepository
    {
        public VoteRepository(EVote470PlusContext context) : base(context)
        {
            
        }

        public async Task<int> GetTotalVotesByElectionAsync(int electionId)
        {
            return await _context.Votes
                .Where(v => v.ElectionId == electionId)
                .Select(v => v.CitizenId)
                .Distinct()
                .CountAsync();
        }

        public async Task<Vote?> GetVoteByCitizenPositionAsync(int citizenId, int electonId, int positionId)
        {
            return await _context.Votes
                .FirstOrDefaultAsync(v => 
                v.CitizenId == citizenId &&
                v.ElectionId == electonId &&
                v.PoliticalPositionId == positionId);
        }

        public async Task<int> GetVoteCountByCandidateAsync(int candidateId, int electionId)
        {
            return await _context.Votes
                .CountAsync(v => v.CandidateId == candidateId && v.ElectionId == electionId);
        }

        public async Task<List<int>> GetVotedPositionIdsAsync(int citizenId, int electionId)
        {
            return await _context.Votes
                .Where(v => v.CitizenId == citizenId && v.ElectionId == electionId)
                .Select(v => v.PoliticalPositionId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Vote>> GetVotesByPositionAndElectionAsync(int positionId, int electionId)
        {
            return await _context.Votes
                .Where(v => v.PoliticalPositionId == positionId && v.ElectionId == electionId)
                .Include(v => v.Candidate)
                    .ThenInclude(c => c.PoliticalParty)
                .ToListAsync();
        }

        public async Task<List<Vote>> GetVotesWithDetails(int electionId)
        {
            return await _context.Votes
                .Where(v => v.ElectionId == electionId)
                .Include(v => v.Candidate)
                    .ThenInclude(c => c.PoliticalParty)
                .Include(v => v.PoliticalPosition)
                .ToListAsync();
        }

        public async Task<bool> HasVotedInElectionAsync(int citizenId, int electionId)
        {
            return await _context.Votes
                .AnyAsync(v => v.CitizenId == citizenId && v.ElectionId == electionId);
        }

        public async Task<bool> HasCompletedVotingAsync(int citizenId, int electionId)
        {
            var totalPositions = await _context.PoliticalPositions
                .Where(p => p.IsActive)
                .CountAsync();

            var votedPositions = await _context.Votes
                .Where(v => v.CitizenId == citizenId && v.ElectionId == electionId)
                .Select(v => v.PoliticalPositionId)
                .Distinct()
                .CountAsync();

            return votedPositions >= totalPositions;
        }
    }
}
