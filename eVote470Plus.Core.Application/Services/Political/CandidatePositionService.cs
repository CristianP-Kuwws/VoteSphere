using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Political;

namespace eVote470Plus.Core.Application.Services.Political
{
    public class CandidatePositionService : GenericService<CandidatePosition, CandidatePositionDto>, ICandidatePositionService
    {
        private readonly ICandidatePositionRepository _repository;
        private readonly IMapper _mapper;

        public CandidatePositionService(ICandidatePositionRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> CandidateHasPositionInPartyAsync(int candidateId, int partyId)
        {
            try
            {
                return await _repository.CandidateHasPositionInPartyAsync(candidateId, partyId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking if candidate {candidateId} has a position in party {partyId}.", ex);
            }
        }

        public async Task<List<CandidatePositionDto>> GetAssignedCandidatesInPartyAsync(int partyId)
        {
            try
            {
                var positions = await _repository.GetAssignedCandidateIdsInPartyAsync(partyId);

                if (positions == null || !positions.Any())
                    return new List<CandidatePositionDto>();

                return _mapper.Map<List<CandidatePositionDto>>(positions);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving assigned candidates in party {partyId}.", ex);
            }
        }

        public async Task<List<CandidatePositionDto>> GetByPartyAsync(int partyId)
        {
            try
            {
                var positions = await _repository.GetByPartyAsync(partyId);

                if (positions == null || !positions.Any())
                    return new List<CandidatePositionDto>();

                return _mapper.Map<List<CandidatePositionDto>>(positions);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving candidate positions for party {partyId}.", ex);
            }
        }

        public async Task<List<CandidatePositionDto>> GetByPositionAsync(int positionId)
        {
            try
            {
                var positions = await _repository.GetByPositionAsync(positionId);

                if (positions == null || !positions.Any())
                    return new List<CandidatePositionDto>();

                return _mapper.Map<List<CandidatePositionDto>>(positions);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving candidate positions for position {positionId}.", ex);
            }
        }

        public async Task<CandidatePositionDto?> GetCandidatePositionInOriginPartyAsync(int candidateId)
        {
            try
            {
                var position = await _repository.GetCandidatePositionInOriginPartyAsync(candidateId);

                if (position == null)
                    return null;

                return _mapper.Map<CandidatePositionDto>(position);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the origin party position for candidate {candidateId}.", ex);
            }
        }
    }
}