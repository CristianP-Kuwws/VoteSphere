using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Interfaces.Relations;
using eVote470Plus.Core.Application.ViewModels.People.Candidate;
using eVote470Plus.Core.Application.ViewModels.Political.CandidatePosition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "PoliticalLeader")]
    public class CandidatePositionAssignmentController : Controller
    {
        private readonly ICandidatePositionService _candidatePositionService;
        private readonly ICandidateService _candidateService;
        private readonly IPoliticalPositionService _politicalPositionService;
        private readonly IPoliticalAllianceService _allianceService;
        private readonly IPoliticalLeaderService _politicalLeaderService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public CandidatePositionAssignmentController(
            ICandidatePositionService candidatePositionService,
            ICandidateService candidateService,
            IPoliticalPositionService politicalPositionService,
            IPoliticalAllianceService allianceService,
            IPoliticalLeaderService politicalLeaderService,
            IElectionService electionService,
            IMapper mapper)
        {
            _candidatePositionService = candidatePositionService;
            _candidateService = candidateService;
            _politicalPositionService = politicalPositionService;
            _allianceService = allianceService;
            _politicalLeaderService = politicalLeaderService;
            _electionService = electionService;
            _mapper = mapper;
        }

        private async Task<bool> HasActiveElectionAsync()
            => await _electionService.HasActiveElectionAsync();

        private async Task<int?> GetCurrentLeaderPartyIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return null;

            var leader = await _politicalLeaderService.GetByIdentityUserIdAsync(userId);
            return leader?.PoliticalPartyId;
        }

        public async Task<IActionResult> Index()
        {
            var partyId = await GetCurrentLeaderPartyIdAsync();
            if (partyId == null)
            {
                TempData["Error"] = "You have no political party assigned.";
                return RedirectToAction("Index", "Home");
            }

            var assignments = await _candidatePositionService.GetByPartyAsync(partyId.Value);
            var vms = new List<CandidatePositionViewModel>();

            foreach (var assignment in assignments)
            {
                var candidate = await _candidateService.GetByIdAsync(assignment.CandidateId);
                var position = await _politicalPositionService.GetByIdAsync(assignment.PoliticalPositionId);

                // Only show assignments where both candidate and position are active 

                vms.Add(new CandidatePositionViewModel
                {
                    CandidatePositionId = assignment.CandidatePositionId,
                    CandidateId = assignment.CandidateId,
                    PoliticalPositionId = assignment.PoliticalPositionId,
                    PoliticalPartyId = assignment.PoliticalPartyId,
                    CandidateName = candidate != null ? $"{candidate.Name} {candidate.LastName}" : "Unknown",
                    PoliticalPositionName = position?.Name ?? "Unknown",
                    CandidateIsActive = candidate?.IsActive ?? false,
                    PositionIsActive = position?.IsActive ?? false
                });
            }

            ViewBag.HasActiveElection = await HasActiveElectionAsync();
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create an assignment while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentLeaderPartyIdAsync();
            if (partyId == null)
            {
                TempData["Error"] = "You have no political party assigned.";
                return RedirectToAction("Index", "Home");
            }

            await PopulateViewBagAsync(partyId.Value);
            return View("Save", new SaveCandidatePositionViewModel
            {
                PoliticalPartyId = partyId.Value
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveCandidatePositionViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create an assignment while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentLeaderPartyIdAsync();
            if (partyId == null)
            {
                TempData["Error"] = "You have no political party assigned.";
                return RedirectToAction("Index", "Home");
            }

            vm.PoliticalPartyId = partyId.Value;

            // Candidate not already assigned in this party
            bool alreadyAssigned = await _candidatePositionService.CandidateHasPositionInPartyAsync(vm.CandidateId, partyId.Value);
            if (alreadyAssigned)
            {
                ModelState.AddModelError(nameof(vm.CandidateId), "This candidate is already assigned to a position within this party.");
            }
            else
            {
                // If candidate is from an allied party, they must aspire to the same position
                var originPosition = await _candidatePositionService.GetCandidatePositionInOriginPartyAsync(vm.CandidateId);
                if (originPosition != null && originPosition.PoliticalPartyId != partyId.Value)
                {
                    // Candidate is from an allied party, position must match
                    if (originPosition.PoliticalPositionId != vm.PoliticalPositionId)
                    {
                        ModelState.AddModelError(nameof(vm.PoliticalPositionId),
                            "This candidate aspires to a different position in their origin party. The position must match.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewBagAsync(partyId.Value);
                return View("Save", vm);
            }

            var dto = _mapper.Map<CandidatePositionDto>(vm);
            await _candidatePositionService.AddAsync(dto);

            TempData["Success"] = "Candidate assigned to position successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot delete an assignment while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var assignment = await _candidatePositionService.GetByIdAsync(id);
            if (assignment == null) return RedirectToAction(nameof(Index));

            var candidate = await _candidateService.GetByIdAsync(assignment.CandidateId);
            var position = await _politicalPositionService.GetByIdAsync(assignment.PoliticalPositionId);

            var vm = new CandidatePositionViewModel
            {
                CandidatePositionId = assignment.CandidatePositionId,
                CandidateId = assignment.CandidateId,
                PoliticalPositionId = assignment.PoliticalPositionId,
                PoliticalPartyId = assignment.PoliticalPartyId,
                CandidateName = candidate != null ? $"{candidate.Name} {candidate.LastName}" : "Unknown",
                PoliticalPositionName = position?.Name ?? "Unknown"
            };

            return View(vm); // Delete.cshtml shows confirmation
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool confirm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot delete an assignment while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _candidatePositionService.DeleteAsync(id);

            TempData["Success"] = "Assignment removed successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        //Populates ViewBag with available candidates and positions for the current party.
        //Respects alliance rules and single-position-per-candidate-per-party constraint.
        private async Task PopulateViewBagAsync(int partyId)
        {
            var assignedInParty = await _candidatePositionService.GetAssignedCandidatesInPartyAsync(partyId);
            var assignedCandidateIds = assignedInParty.Select(a => a.CandidateId).ToHashSet();

            // Own party candidates not yet assigned
            var ownCandidates = await _candidateService.GetByPoliticalPartyAsync(partyId);
            var availableOwnCandidates = ownCandidates
                .Where(c => c.IsActive && !assignedCandidateIds.Contains(c.CandidateId))
                .ToList();

            // Allied party candidates — working in DTOs to avoid type mismatch
            var alliedCandidates = new List<CandidateDto>();
            var activeAlliances = await _allianceService.GetActiveAlliancesAsync(partyId);

            foreach (var alliance in activeAlliances)
            {
                int alliedPartyId = alliance.SenderPartyId == partyId
                    ? alliance.ReceiverPartyId
                    : alliance.SenderPartyId;

                var alliedPartyCandidates = await _candidateService.GetByPoliticalPartyAsync(alliedPartyId);

                foreach (var candidate in alliedPartyCandidates.Where(c => c.IsActive))
                {
                    if (assignedCandidateIds.Contains(candidate.CandidateId))
                        continue;

                    var originPosition = await _candidatePositionService.GetCandidatePositionInOriginPartyAsync(candidate.CandidateId);
                    if (originPosition != null)
                        alliedCandidates.Add(candidate);
                }
            }

            // Now both lists are List<CandidateDto> — Concat works fine
            var availableCandidates = availableOwnCandidates
                .Concat(alliedCandidates)
                .DistinctBy(c => c.CandidateId)
                .ToList();

            var allActivePositions = await _politicalPositionService.GetActivePositionsAsync();
            var filledPositionIds = assignedInParty
                .Where(a => allActivePositions.Any(p => p.PoliticalPositionId == a.PoliticalPositionId))
                .Select(a => a.PoliticalPositionId)
                .ToHashSet();
            var availablePositions = allActivePositions
                .Where(p => !filledPositionIds.Contains(p.PoliticalPositionId))
                .ToList();

            // Map to ViewModels only at the end before passing to ViewBag
            ViewBag.AvailableCandidates = _mapper.Map<List<CandidateViewModel>>(availableCandidates);
            ViewBag.AvailablePositions = availablePositions; // List<PoliticalPositionDto>
        }
    }
}
