using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Interfaces.Relations;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Common.Enum;
using eVote470Plus.Core.Domain.Entities.Relations;
using eVote470Plus.Core.Domain.Interfaces.Relations;

namespace eVote470Plus.Core.Application.Services.Relations
{
    public class PoliticalAllianceService : GenericService<PoliticalAlliance, PoliticalAllianceDto>, IPoliticalAllianceService
    {
        private readonly IPoliticalAllianceRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICandidatePositionService _candidatePositionService;
        private readonly ICandidateService _candidateService;

        public PoliticalAllianceService(
            IPoliticalAllianceRepository repository,
            IMapper mapper,
            ICandidatePositionService candidatePositionService,
            ICandidateService candidateService)
            : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _candidatePositionService = candidatePositionService;
            _candidateService = candidateService;
        }

        public override async Task<PoliticalAllianceDto?> AddAsync(PoliticalAllianceDto dtoModel)
        {
            try
            {
                bool hasPending = await _repository.HasPendingRequestBetweenPartiesAsync(dtoModel.SenderPartyId, dtoModel.ReceiverPartyId);

                if (hasPending)
                {
                    throw new InvalidOperationException("There is already a pending request between these parties.");
                }

                dtoModel.Status = AllianceStatus.Pending;
                dtoModel.RequestDate = DateTime.UtcNow;
                dtoModel.ResponseDate = null;

                return await base.AddAsync(dtoModel);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the alliance request.", ex);

            }
        }

        public async Task<List<PoliticalAllianceDto>> GetActiveAlliancesAsync(int partyId)
        {
            try
            {
                var alliances = await _repository.GetActiveAlliancesAsync(partyId);

                if (alliances == null || !alliances.Any())
                {
                    return new List<PoliticalAllianceDto>();
                }

                return _mapper.Map<List<PoliticalAllianceDto>>(alliances);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while obtaining active Alliances.", ex);
            }
        }

        public async Task<List<PoliticalAllianceDto>> GetPendingReceivedRequestsAsync(int partyId)
        {
            try
            {
                var alliances = await _repository.GetPendingReceivedRequestsAsync(partyId);

                if (alliances == null || !alliances.Any())
                {
                    return new List<PoliticalAllianceDto>();
                }

                return _mapper.Map<List<PoliticalAllianceDto>>(alliances);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while obtaining pending Alliance Requests.", ex);
            }
        }

        public async Task<List<PoliticalAllianceDto>> GetSentRequestsAsync(int partyId)
        {
            try
            {
                var alliances = await _repository.GetSentRequestsAsync(partyId);

                if (alliances == null || !alliances.Any())
                {
                    return new List<PoliticalAllianceDto>();
                }

                return _mapper.Map<List<PoliticalAllianceDto>>(alliances);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while obtaining sent Alliance Requests.", ex);
            }
        }

        public async Task<bool> HasPendingRequestBetweenPartiesAsync(int party1Id, int party2Id)
        {
            try
            {
                return await _repository.HasPendingRequestBetweenPartiesAsync(party1Id, party2Id);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while obtaining pending requests between parties.", ex);
            }
        }

        public async Task AcceptAllianceAsync(int allianceId)
        {
            try
            {
                var alliance = await _repository.GetByIdAsync(allianceId);

                if (alliance == null)
                {
                    throw new KeyNotFoundException("Alliance request not found.");
                }

                if (alliance.Status != AllianceStatus.Pending)
                {
                    throw new InvalidOperationException("Only pending requests can be accepted.");
                }

                alliance.Status = AllianceStatus.Accepted;
                alliance.ResponseDate = DateTime.UtcNow;

                await _repository.UpdateAsync(allianceId, alliance);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while accepting the alliance request.", ex);
            }
        }

        public async Task RejectAllianceAsync(int allianceId)
        {
            try
            {
                var alliance = await _repository.GetByIdAsync(allianceId);

                if (alliance == null)
                {
                    throw new KeyNotFoundException("Alliance request not found.");
                }

                if (alliance.Status != AllianceStatus.Pending)
                {
                    throw new InvalidOperationException("Only pending requests can be rejected.");
                }

                alliance.Status = AllianceStatus.Rejected;
                alliance.ResponseDate = DateTime.UtcNow;

                await _repository.UpdateAsync(allianceId, alliance);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while rejecting the alliance request.", ex);

            }

        }

        public async Task EndAllianceAsync(int allianceId, int currentPartyId)
        {
            try
            {
                var alliance = await _repository.GetByIdAsync(allianceId);

                if (alliance == null)
                    throw new KeyNotFoundException("Alliance not found.");

                if (alliance.Status != AllianceStatus.Accepted)
                    throw new InvalidOperationException("Only active alliances can be ended.");

                if (alliance.SenderPartyId != currentPartyId &&
                    alliance.ReceiverPartyId != currentPartyId)
                {
                    throw new UnauthorizedAccessException("You are not part of this alliance.");
                }

                await RemoveCrossPartyCandidatesAsync(
                    alliance.SenderPartyId,
                    alliance.ReceiverPartyId
                );

                await _repository.DeleteAsync(allianceId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while ending the alliance.", ex);
            }
        }

        // IMPORTANT:
        // Ending an alliance requires cleaning up cross-party candidate assignments.
        // This is coordinated here to keep business logic consistent.

        private async Task RemoveCrossPartyCandidatesAsync(int partyAId, int partyBId)
        {
            var partyAPositions = await _candidatePositionService.GetByPartyAsync(partyAId);
            var partyBPositions = await _candidatePositionService.GetByPartyAsync(partyBId);

            var allPositions = partyAPositions.Concat(partyBPositions).ToList();

            var partyACandidates = await _candidateService.GetByPoliticalPartyAsync(partyAId);
            var partyBCandidates = await _candidateService.GetByPoliticalPartyAsync(partyBId);

            var allCandidates = partyACandidates
                .Concat(partyBCandidates)
                .ToDictionary(c => c.CandidateId);

            foreach (var position in allPositions)
            {
                if (!allCandidates.TryGetValue(position.CandidateId, out var candidate))
                    continue;

                if (candidate.PoliticalPartyId != position.PoliticalPartyId)
                {
                    await _candidatePositionService.DeleteAsync(position.CandidatePositionId);
                }
            }
        }
    }
}
