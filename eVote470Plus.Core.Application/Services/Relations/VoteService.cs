using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Application.Interfaces.Relations;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Entities.Relations;
using eVote470Plus.Core.Domain.Interfaces.Relations;

namespace eVote470Plus.Core.Application.Services.Relations
{
    public class VoteService : GenericService<Vote, VoteDto>, IVoteService
    {
        private readonly IVoteRepository _repository;
        private readonly IMapper _mapper;
        public VoteService(IVoteRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task CastVoteAsync(int citizenId, int electionId, int candidateId, int positionId)
        {
            try
            {
                var existingVote = await _repository.GetVoteByCitizenPositionAsync(citizenId, electionId, positionId);

                if (existingVote != null)
                {
                    throw new InvalidOperationException("Citizen has already voted for this position in this election.");
                }

                var vote = new Vote
                {
                    CitizenId = citizenId,
                    ElectionId = electionId,
                    CandidateId = candidateId == 0 ? (int?)null : candidateId,
                    PoliticalPositionId = positionId,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(vote);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while casting the vote.", ex);
            }
        }

        public async Task<int> GetTotalVotesByElectionAsync(int electionId)
        {
            try
            {
                var totalElectionVotes = await _repository.GetTotalVotesByElectionAsync(electionId);
                return totalElectionVotes;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while obtaining the total of votes by Election.", ex);
                
            }
        }

        public async Task<int> GetVoteCountByCandidateAsync(int candidateId, int electionId)
        {
            try
            {
                var totalElectionVotes = await _repository.GetVoteCountByCandidateAsync(candidateId, electionId);
                return totalElectionVotes;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while obtaining the count of votes by Candidate.", ex);

            }
        }

        public async Task<List<int>> GetVotedPositionIdsAsync(int citizenId, int electionId)
        {
            try
            {
                return await _repository.GetVotedPositionIdsAsync(citizenId, electionId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting voted positions.", ex);
            }
        }

        public async Task<List<VoteDto>> GetVotesByPositionAndElectionAsync(int positionId, int electionId)
        {
            try
            {
                var totalElectionVotes = await _repository.GetVotesByPositionAndElectionAsync(positionId, electionId);
                return _mapper.Map<List<VoteDto>>(totalElectionVotes);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while obtaining votes by position and election.", ex);

            }
        }

        public async Task<bool> HasVotedInElectionAsync(int citizenId, int electionId)
        {
            try
            {
                return await _repository.HasVotedInElectionAsync(citizenId, electionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while trying to confirm if Citizen with ID: {citizenId}, has voted in Election with ID: {electionId}");
            }

        }

        public async Task<bool> HasCompletedVotingAsync(int citizenId, int electionId)
        {
            try
            {
                return await _repository.HasCompletedVotingAsync(citizenId, electionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while trying to confirm if Citizen with ID: {citizenId}, has completed voting in Election with ID: {electionId}", ex);
            }

        }
    }
}
