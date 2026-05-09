using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Interfaces.Relations;
using eVote470Plus.Core.Application.ViewModels.Political.PoliticalParty;
using eVote470Plus.Core.Application.ViewModels.Relations.PoliticalAlliance;
using eVote470Plus.Core.Domain.Common.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "PoliticalLeader")]
    public class PoliticalAllianceController : Controller
    {
        private readonly IPoliticalAllianceService _allianceService;
        private readonly IPoliticalPartyService _politicalPartyService;
        private readonly IPoliticalLeaderService _politicalLeaderService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public PoliticalAllianceController(
            IPoliticalAllianceService allianceService,
            IPoliticalPartyService politicalPartyService,
            IPoliticalLeaderService politicalLeaderService,
            IElectionService electionService,
            IMapper mapper)
        {
            _allianceService = allianceService;
            _politicalPartyService = politicalPartyService;
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

            // Fetch all three lists
            var pendingReceived = await _allianceService.GetPendingReceivedRequestsAsync(partyId.Value);
            var sentRequests = await _allianceService.GetSentRequestsAsync(partyId.Value);
            var activeAlliances = await _allianceService.GetActiveAlliancesAsync(partyId.Value);

            // Map and enrich with party names
            var pendingReceivedVms = await EnrichAllianceViewModelsAsync(_mapper.Map<List<PoliticalAllianceViewModel>>(pendingReceived));
            var sentRequestsVms = await EnrichAllianceViewModelsAsync(_mapper.Map<List<PoliticalAllianceViewModel>>(sentRequests)); //  error here
            var activeAlliancesVms = await EnrichAllianceViewModelsAsync(_mapper.Map<List<PoliticalAllianceViewModel>>(activeAlliances));

            ViewBag.PendingReceived = pendingReceivedVms;
            ViewBag.SentRequests = sentRequestsVms;
            ViewBag.ActiveAlliances = activeAlliancesVms;
            ViewBag.HasActiveElection = await HasActiveElectionAsync();
            ViewBag.CurrentPartyId = partyId.Value;

            return View();
        }

        public async Task<IActionResult> Create()
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create an alliance request while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentLeaderPartyIdAsync();
            if (partyId == null)
            {
                TempData["Error"] = "You have no political party assigned.";
                return RedirectToAction("Index", "Home");
            }

            await PopulateAvailablePartiesAsync(partyId.Value);

            return View("Save", new SavePoliticalAllianceViewModel
            {
                SenderPartyId = partyId.Value
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePoliticalAllianceViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create an alliance request while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentLeaderPartyIdAsync();
            if (partyId == null)
            {
                TempData["Error"] = "You have no political party assigned.";
                return RedirectToAction("Index", "Home");
            }

            vm.SenderPartyId = partyId.Value;

            // Validate no pending request exists between these two parties
            bool hasPending = await _allianceService.HasPendingRequestBetweenPartiesAsync(vm.SenderPartyId, vm.ReceiverPartyId);
            if (hasPending)
                ModelState.AddModelError(nameof(vm.ReceiverPartyId), "There is already a pending request between these parties.");

            if (!ModelState.IsValid)
            {
                await PopulateAvailablePartiesAsync(partyId.Value);
                return View("Save", vm);
            }

            var dto = _mapper.Map<PoliticalAllianceDto>(vm);
            await _allianceService.AddAsync(dto);

            TempData["Success"] = "Alliance request sent successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Accept(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot accept an alliance request while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _allianceService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<PoliticalAllianceViewModel>(dto);
            await EnrichSingleViewModelAsync(vm);
            return View(vm); // Accept.cshtml shows ¿Está seguro? confirmation
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id, bool confirm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot accept an alliance request while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _allianceService.AcceptAllianceAsync(id);

            TempData["Success"] = "Alliance accepted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot reject an alliance request while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _allianceService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<PoliticalAllianceViewModel>(dto);
            await EnrichSingleViewModelAsync(vm);
            return View(vm); // Reject.cshtml shows ¿Está seguro? confirmation
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, bool confirm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot reject an alliance request while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _allianceService.RejectAllianceAsync(id);

            TempData["Success"] = "Alliance request rejected.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot delete an alliance request while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _allianceService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<PoliticalAllianceViewModel>(dto);
            await EnrichSingleViewModelAsync(vm);
            return View(vm); // Delete.cshtml shows ¿Está seguro? confirmation
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool confirm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot delete an alliance request while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _allianceService.DeleteAsync(id);

            TempData["Success"] = "Alliance request deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EndAlliance(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot end an alliance while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentLeaderPartyIdAsync();
            if (partyId == null)
            {
                TempData["Error"] = "You have no political party assigned.";
                return RedirectToAction("Index", "Home");
            }

            var dto = await _allianceService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<PoliticalAllianceViewModel>(dto);
            await EnrichSingleViewModelAsync(vm);

            ViewBag.CurrentPartyId = partyId.Value;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndAlliance(int id, bool confirm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot end an alliance while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentLeaderPartyIdAsync();
            if (partyId == null)
            {
                TempData["Error"] = "You have no political party assigned.";
                return RedirectToAction("Index", "Home");
            }

            await _allianceService.EndAllianceAsync(id, partyId.Value);

            TempData["Success"] = "Alliance ended successfully.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Populates ViewBag with active parties that have no pending request with the current party.
        /// </summary>
        private async Task PopulateAvailablePartiesAsync(int currentPartyId)
        {
            var allParties = await _politicalPartyService.GetAllAsync();
            var sentRequests = await _allianceService.GetSentRequestsAsync(currentPartyId);
            var receivedRequests = await _allianceService.GetPendingReceivedRequestsAsync(currentPartyId);

            // Party IDs that already have a pending request in either direction
            var blockedPartyIds = sentRequests
                .Where(r => r.Status == AllianceStatus.Pending)
                .Select(r => r.ReceiverPartyId)
                .Union(receivedRequests.Select(r => r.SenderPartyId))
                .ToHashSet();

            var availableParties = allParties
                .Where(p => p.IsActive && p.PoliticalPartyId != currentPartyId && !blockedPartyIds.Contains(p.PoliticalPartyId))
                .ToList();

            ViewBag.AvailableParties = _mapper.Map<List<PoliticalPartyViewModel>>(availableParties);
        }

        /// <summary>
        /// Enriches a list of ViewModels with sender and receiver party names.
        /// </summary>
        private async Task<List<PoliticalAllianceViewModel>> EnrichAllianceViewModelsAsync(List<PoliticalAllianceViewModel> vms)
        {
            foreach (var vm in vms)
                await EnrichSingleViewModelAsync(vm);
            return vms;
        }

        /// <summary>
        /// Enriches a single ViewModel with sender and receiver party names and acronyms.
        /// </summary>
        private async Task EnrichSingleViewModelAsync(PoliticalAllianceViewModel vm)
        {
            var senderParty = await _politicalPartyService.GetByIdAsync(vm.SenderPartyId);
            var receiverParty = await _politicalPartyService.GetByIdAsync(vm.ReceiverPartyId);

            vm.SenderPartyName = senderParty != null ? $"{senderParty.Name} [{senderParty.Acronym}]" : "Unknown";
            vm.ReceiverPartyName = receiverParty != null ? $"{receiverParty.Name} [{receiverParty.Acronym}]" : "Unknown";
        }
    }
}
