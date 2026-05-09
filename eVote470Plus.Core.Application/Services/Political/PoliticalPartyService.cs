using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Political;

namespace eVote470Plus.Core.Application.Services.Political
{
    public class PoliticalPartyService : GenericService<PoliticalParty, PoliticalPartyDto>, IPoliticalPartyService
    {
        private readonly IPoliticalPartyRepository _repository;
        private readonly IMapper _mapper;

        public PoliticalPartyService(IPoliticalPartyRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> ExistAcronymAsync(string acronym, int? excludeId = null)
        {
            try
            {
                var politicalParty = await _repository.GetByAcronymAsync(acronym);

                if (politicalParty == null)
                {
                    return false;
                }

                if (excludeId.HasValue)
                {
                    return politicalParty.PoliticalPartyId != excludeId.Value;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while trying to obtain the Acronym.", ex);

            }
        }

        public async Task<List<PoliticalPartyDto>> GetActivePartiesWithCandidatesAsync()
        {
            try
            {
                var operation = await _repository.GetActivePartiesWithCandidatesAsync();
                return _mapper.Map<List<PoliticalPartyDto>>(operation);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while trying to obtain the Active Parties of the Political Party.", ex);
            }
        }

        public async Task ToggleActiveAsync(int id)
        {
            try
            {
                var politicalParty = await _repository.GetByIdAsync(id);

                if (politicalParty == null)
                {
                    throw new KeyNotFoundException("Political Party not found.");
                }

                bool newStatus = !politicalParty.IsActive;
                bool success = await _repository.ChangeActiveStatusAsync(id, newStatus);

                if (!success)
                {
                    throw new Exception("Failed to change active status.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while toggling the active status of the Political Party.", ex);
            }
        }
        
    }
}
