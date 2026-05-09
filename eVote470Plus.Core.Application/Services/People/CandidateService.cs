using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.People;

namespace eVote470Plus.Core.Application.Services.People
{
    public class CandidateService : GenericService<Candidate, CandidateDto>, ICandidateService
    {
        private readonly ICandidateRepository _repository;
        private readonly IMapper _mapper;

        public CandidateService(ICandidateRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> ExistsActiveCandidateForPartyAndPositionAsync(int partyId, int positionId)
        {
            try
            {
                var operation = await _repository.ExistsActiveCandidateForPartyAndPositionAsync(partyId, positionId);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while obtaining existing Candidates for Political Party ID: {partyId} and Position ID: {positionId}.", ex);
            }
        }

        public async Task<List<CandidateDto>> GetByPoliticalPartyAsync(int partyId)
        {
            try
            {
                var candidates = await _repository.GetByPoliticalPartyAsync(partyId);

                if (candidates == null || !candidates.Any())
                {
                    return new List<CandidateDto>();
                }

                return _mapper.Map<List<CandidateDto>>(candidates);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while obtaining Candidates by Political Party {partyId}.", ex);
            }
        }

        public async Task<List<CandidateDto>> GetByPositionAsync(int positionId)
        {
            try
            {
                var candidates = await _repository.GetByPositionAsync(positionId);

                if (candidates == null || !candidates.Any())
                {
                    return new List<CandidateDto>();
                }

                return _mapper.Map<List<CandidateDto>>(candidates);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while obtaining Candidates by position {positionId}.", ex);
            }
        }

        public async Task ToggleActiveAsync(int candidateId)
        {
            try
            {
                var candidate = await _repository.GetByIdAsync(candidateId);

                if (candidate == null)
                {
                    throw new KeyNotFoundException("Candidate not found.");
                }

                bool newStatus = !candidate.IsActive;
                bool success = await _repository.ToggleActiveAsync(candidateId, newStatus);

                if (!success)
                {
                    throw new Exception("Failed to change active status.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while toggling the active status of the Candidate.", ex);
            }
        }
    }
}
