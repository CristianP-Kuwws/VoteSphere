using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.People;

namespace eVote470Plus.Core.Application.Services.People
{
    public class CitizenService : GenericService<Citizen, CitizenDto>, ICitizenService
    {
        private readonly ICitizenRepository _repository;
        private readonly IMapper _mapper;

        public CitizenService(ICitizenRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> ExistsDocumentNumberAsync(string documentNumber, int? excludeId = null)
        {
            try
            {
                var citizen = await _repository.GetByDocumentNumberAsync(documentNumber);

                if (citizen == null)
                {
                    return false;
                }

                if (excludeId.HasValue)
                {
                    return citizen.CitizenId != excludeId.Value; // If it's the same we want to exclude, it doesn't exist
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking the document number existence.", ex);
            }
        }

        public async Task<CitizenDto?> GetByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                var citizen = await _repository.GetByDocumentNumberAsync(documentNumber);

                if (citizen == null)
                {
                    return null;
                }

                return _mapper.Map<CitizenDto>(citizen);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the citizen by document number.", ex);
            }
        }

        public async Task ToggleActiveAsync(int citizenId)
        {
            try
            {
                var citizen = await _repository.GetByIdAsync(citizenId);

                if (citizen == null)
                {
                    throw new Exception("Citizen not found.");
                }

                citizen.IsActive = !citizen.IsActive;

                await _repository.UpdateAsync(citizen.CitizenId, citizen);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while toggling the active status of the citizen.", ex);

            }
        }
    }
}
