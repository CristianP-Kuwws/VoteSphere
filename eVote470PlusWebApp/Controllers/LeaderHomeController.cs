using AutoMapper;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Interfaces.Relations;
using eVote470Plus.Core.Application.ViewModels.Political.PoliticalParty;
using eVote470PlusWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "PoliticalLeader")]
    public class LeaderHomeController : Controller
    {
        private readonly IPoliticalLeaderService _politicalLeaderService;
        private readonly IPoliticalPartyService _politicalPartyService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidatePositionService _candidatePositionService;
        private readonly IPoliticalAllianceService _allianceService;
        private readonly IMapper _mapper;

        public LeaderHomeController(
            IPoliticalLeaderService politicalLeaderService,
            IPoliticalPartyService politicalPartyService,
            ICandidateService candidateService,
            ICandidatePositionService candidatePositionService,
            IPoliticalAllianceService allianceService,
            IMapper mapper)
        {
            _politicalLeaderService = politicalLeaderService;
            _politicalPartyService = politicalPartyService;
            _candidateService = candidateService;
            _candidatePositionService = candidatePositionService;
            _allianceService = allianceService;
            _mapper = mapper;
        }

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
                return RedirectToAction("Index", "Login");
            }

            var partyDto = await _politicalPartyService.GetByIdAsync(partyId.Value);
            if (partyDto == null)
                return RedirectToAction("Index", "Login");

            var partyVm = _mapper.Map<PoliticalPartyViewModel>(partyDto);

            // Indicators
            var allCandidates = await _candidateService.GetByPoliticalPartyAsync(partyId.Value);
            int activeCandidates = allCandidates.Count(c => c.IsActive);
            int inactiveCandidates = allCandidates.Count(c => !c.IsActive);

            var activeAlliances = await _allianceService.GetActiveAlliancesAsync(partyId.Value);
            var pendingRequests = await _allianceService.GetPendingReceivedRequestsAsync(partyId.Value);

            var assignedCandidates = await _candidatePositionService.GetByPartyAsync(partyId.Value);

            ViewBag.Party = partyVm;
            ViewBag.ActiveCandidates = activeCandidates;
            ViewBag.InactiveCandidates = inactiveCandidates;
            ViewBag.AllianceCount = activeAlliances.Count;
            ViewBag.PendingRequestCount = pendingRequests.Count;
            ViewBag.AssignedCandidateCount = assignedCandidates.Count;

            return View();
        }
    }
}
