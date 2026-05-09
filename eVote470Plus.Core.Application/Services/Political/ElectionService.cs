using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Dtos.Results;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.People;
using eVote470Plus.Core.Domain.Interfaces.Political;
using eVote470Plus.Core.Domain.Interfaces.Relations;

namespace eVote470Plus.Core.Application.Services.Political
{
    public class ElectionService : GenericService<Election, ElectionDto>, IElectionService
    {
        private readonly IElectionRepository _repository;
        private readonly IVoteRepository _voteRepository;
        private readonly IPoliticalPositionRepository _politicalPositionRepository;
        private readonly IPoliticalPartyRepository _politicalPartyRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICitizenRepository _citizenRepository; 
        private readonly IMapper _mapper;

        public ElectionService(
            IElectionRepository repository,
            IVoteRepository voteRepository,
            IPoliticalPositionRepository politicalPositionRepository,
            IPoliticalPartyRepository politicalPartyRepository,
            ICandidateRepository candidateRepository,
            ICitizenRepository citizenRepository,
            IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _voteRepository = voteRepository;
            _politicalPositionRepository = politicalPositionRepository;
            _politicalPartyRepository = politicalPartyRepository;
            _candidateRepository = candidateRepository;
            _citizenRepository = citizenRepository;
            _mapper = mapper;
        }

        public async Task<ElectionValidationResult> CanCreateElection()
        {
            var result = new ElectionValidationResult { CanCreate = true };

            try
            {
                if (await _repository.HasActiveElectionAsync())
                {
                    result.CanCreate = false;
                    result.ValidationErrors.Add("There's already an active election.");
                    return result;
                }

                var activePositions = await _politicalPositionRepository.GetActivePositionsAsync();

                if (activePositions == null || !activePositions.Any())
                {
                    result.CanCreate = false;
                    result.ValidationErrors.Add("There is no political positions");
                    return result;
                }

                var activeParties = await _politicalPartyRepository.GetActivePartiesAsync(); // Faltante

                if (activeParties == null || activeParties.Count < 2)
                {
                    result.CanCreate = false;
                    result.ValidationErrors.Add("There are not enough political parties for an election.");
                    return result;
                }

                foreach (var party in activeParties)
                {
                    var missingPositions = new List<string>();

                    foreach (var position in activePositions)
                    {
                        bool hasCandidate = await _candidateRepository.ExistsActiveCandidateForPartyAndPositionAsync(party.PoliticalPartyId, position.PoliticalPositionId);

                        if (!hasCandidate)
                        {
                            missingPositions.Add(position.Name);
                        }
                    }

                    if (missingPositions.Any())
                    {
                        result.CanCreate = false;
                        result.ValidationErrors.Add(
                        $"The political party {party.Name} [{party.Acronym}] doesn't have registered candidates for the following political positions: {string.Join(", ", missingPositions)}."
                        );

                    }
                
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while validating election creation.", ex);

            }
        }

        public async Task<bool> ChangeActiveStatusAsync(int electionId)
        {
            try
            {
                var election = await _repository.GetByIdAsync(electionId);

                if (election == null)
                {
                    throw new KeyNotFoundException("Election not found.");
                }

                var newStatus = !election.IsActive;

                return await _repository.ChangeActiveStatusAsync(electionId, newStatus);

            }
            catch (Exception ex) 
            {
                throw new Exception("An error occurred while toggling the active status of the Election.", ex);
            }
        }

        public async Task FinalizeElectionAsync(int electionId)
        {
            try
            {
                var election = await _repository.GetByIdAsync(electionId);

                if (election == null)
                {
                    throw new KeyNotFoundException("Election not found.");
                }

                if (!election.IsActive)
                {
                    throw new InvalidOperationException("Election is already finalized.");
                }

                await _repository.ChangeActiveStatusAsync(electionId, false);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while finalizing the election.", ex);
            }
        }

        public async Task<ElectionDto?> GetActiveElectionAsync()
        {
            try
            {
                var activeElection = await _repository.GetActiveElectionAsync();
                
                if (activeElection == null)
                {
                    return null;
                }

                return _mapper.Map<ElectionDto?>(activeElection);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while trying to obtain active election.", ex);
            }
        }

        public async Task<List<ElectionResultDto>> GetElectionResultsAsync(int electionId)
        {
            try
            {
                var votes = await _voteRepository.GetVotesWithDetails(electionId);

                var resultsByPosition = votes
                    .GroupBy(v => new { v.PoliticalPositionId, PositionName = v.PoliticalPosition.Name })
                    .Select(positionGroup => new ElectionResultDto
                    {
                        PositionId = positionGroup.Key.PoliticalPositionId,
                        PositionName = positionGroup.Key.PositionName,
                        Candidates = positionGroup
                            .GroupBy(v => new
                            {
                                CandidateId = v.CandidateId ?? 0,
                                Name = v.Candidate != null ? v.Candidate.Name : "None",
                                LastName = v.Candidate != null ? v.Candidate.LastName : string.Empty,
                                PhotoUrl = v.Candidate?.PhotoUrl,
                                PartyName = v.Candidate?.PoliticalParty?.Name ?? "N/A",
                                PartyAcronym = v.Candidate?.PoliticalParty?.Acronym ?? "N/A"
                            })
                            .Select(candidateGroup => new CandidateResultDto
                            {
                                CandidateId = candidateGroup.Key.CandidateId, 
                                CandidateName = candidateGroup.Key.Name,
                                CandidateLastName = candidateGroup.Key.LastName,
                                PartyName = candidateGroup.Key.PartyName,
                                PartyAcronym = candidateGroup.Key.PartyAcronym,
                                PhotoUrl = candidateGroup.Key.PhotoUrl,
                                VoteCount = candidateGroup.Count(),
                                VotePercentage = (decimal)candidateGroup.Count() / positionGroup.Count() * 100
                            })
                            .OrderByDescending(c => c.VoteCount)
                            .ToList()
                    })
                    .ToList();

                return resultsByPosition;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting election results.", ex);
            }
        }

        public async Task<List<ElectionDto>> GetElectionsByYearAsync(int year)
        {
            try
            {
                var elections = await _repository.GetElectionsByYearAsync(year);

                if (elections == null || !elections.Any())
                {
                    return new List<ElectionDto>();
                }

                var electionDtos = _mapper.Map<List<ElectionDto>>(elections);

                // Load shared statistics once
                var activeParties = await _politicalPartyRepository.GetActivePartiesAsync();
                var activePositions = await _politicalPositionRepository.GetActivePositionsAsync();
                var activeCitizens = await _citizenRepository.GetAllListAsync();

                int totalEligibleCitizens = activeCitizens.Count(c => c.IsActive);

                foreach (var election in electionDtos)
                {
                    // Political parties count
                    election.ParticipantPoliticalPartiesCount = activeParties.Count;

                    // Political positions count
                    election.PoliticalPositionsCount = activePositions.Count;

                    // Votes / turnout
                    var votes = await _voteRepository.GetVotesWithDetails(election.ElectionId);

                    int uniqueVoters = votes
                        .Select(v => v.CitizenId)
                        .Distinct()
                        .Count();

                    election.VoterTurnoutPercentage =
                        totalEligibleCitizens == 0
                            ? 0
                            : ((decimal)uniqueVoters / totalEligibleCitizens) * 100;
                }

                return electionDtos;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while trying to obtain elections for year {year}.", ex);
            }
        }

        public async Task<List<int>> GetElectionYearsAsync()
        {
            try
            {
                return await _repository.GetElectionYearAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting election years.", ex);
            }
        }

        public async Task<bool> HasActiveElectionAsync()
        {
            try
            {
                return await _repository.HasActiveElectionAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while trying to obtain active elections.", ex);
            }
        }
    }
}
