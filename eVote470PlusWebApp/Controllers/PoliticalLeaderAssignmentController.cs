using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.ApplicationUser;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.ViewModels.People.PoliticalLeader;
using eVote470Plus.Core.Application.ViewModels.Political.PoliticalParty;
using eVote470Plus.Core.Domain.Common.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PoliticalLeaderAssignmentController : Controller
    {
        private readonly IElectionService _electionService;
        private readonly IPoliticalLeaderService _politicalLeaderService;
        private readonly IPoliticalPartyService _politicalPartyService;
        private readonly IAccountServicesForWebApp _accountServicesForWebApp;
        private readonly IMapper _mapper;

        public PoliticalLeaderAssignmentController(
            IElectionService electionService,
            IPoliticalLeaderService politicalLeaderService,
            IPoliticalPartyService politicalPartyService,
            IAccountServicesForWebApp accountServicesForWebApp,
            IMapper mapper)
        {
            _electionService = electionService;
            _politicalLeaderService = politicalLeaderService;
            _politicalPartyService = politicalPartyService;
            _accountServicesForWebApp = accountServicesForWebApp;
            _mapper = mapper;
        }

        private async Task<bool> HasActiveElectionAsync()
            => await _electionService.HasActiveElectionAsync();
        public async Task<IActionResult> Index()
        {
            var assignments = await _politicalLeaderService.GetAllAsync();

            // Build display ViewModels — join with user and party data
            var vms = new List<PoliticalLeaderAssignmentViewModel>();

            foreach (var assignment in assignments)
            {
                var user = await _accountServicesForWebApp.GetUserById(assignment.IdentityUserId);

                var party = await _politicalPartyService.GetByIdAsync(assignment.PoliticalPartyId);


                vms.Add(new PoliticalLeaderAssignmentViewModel
                {
                    PoliticalLeaderId = assignment.PoliticalLeaderId,
                    IdentityUserId = assignment.IdentityUserId,
                    UserName = user != null ? $"{user.Name} {user.LastName}" : "Unknown",
                    PartyAcronym = party?.Acronym ?? "Unknown Party"
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

            await PopulateViewBagAsync();
            return View("Save", new SavePoliticalLeaderAssignmentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePoliticalLeaderAssignmentViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create an assignment while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var existingAssignment = await _politicalLeaderService.GetByIdentityUserIdAsync(vm.IdentityUserId);
            if (existingAssignment != null)
            {
                ModelState.AddModelError(nameof(vm.IdentityUserId), "This leader is already related to another political party.");
            }

            // Validate leader not already assigned
            var existingByLeader = await _politicalLeaderService.GetByIdentityUserIdAsync(vm.IdentityUserId);
            if (existingByLeader != null)
                ModelState.AddModelError(nameof(vm.IdentityUserId), "This leader is already related to another political party.");

            // Validate party doesn't already have a leader
            var allAssignments = await _politicalLeaderService.GetAllAsync();
            if (allAssignments.Any(a => a.PoliticalPartyId == vm.PoliticalPartyId))
                ModelState.AddModelError(nameof(vm.PoliticalPartyId), "This political party already has a leader assigned.");

            if (!ModelState.IsValid)
            {
                await PopulateViewBagAsync();
                return View("Save", vm);
            }

            var dto = new PoliticalLeaderDto
            {
                PoliticalLeaderId = Guid.NewGuid().ToString(),
                IdentityUserId = vm.IdentityUserId,
                PoliticalPartyId = vm.PoliticalPartyId
            };

            await _politicalLeaderService.AddAsync(dto);

            TempData["Success"] = "Assignment created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot delete an assignment while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var assignment = await _politicalLeaderService.GetByStringIdAsync(id);
            if (assignment == null) return RedirectToAction(nameof(Index));

            var user = await _accountServicesForWebApp.GetUserById(assignment.IdentityUserId);
            var party = await _politicalPartyService.GetByIdAsync(assignment.PoliticalPartyId);

            var vm = new PoliticalLeaderAssignmentViewModel
            {
                PoliticalLeaderId = assignment.PoliticalLeaderId,
                IdentityUserId = assignment.IdentityUserId,
                UserName = user != null ? $"{user.Name} {user.LastName}" : "Unknown",
                PartyAcronym = party?.Acronym ?? "Unknown"
            };

            return View(vm); // Delete.cshtml shows confirmation
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, bool currentStatus)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot delete an assignment while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _politicalLeaderService.DeleteByStringIdAsync(id);

            TempData["Success"] = "Assignment deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Populates ViewBag with available leaders (no party assigned) and active parties.
        /// </summary>

        private async Task PopulateViewBagAsync()
        {
            var allUsers = await _accountServicesForWebApp.GetAllAsync(isActive: true); // only active users
            var allRoles = new List<(string userId, string role)>();

            foreach (var user in allUsers)
            {
                var roles = await _accountServicesForWebApp.GetRolesByUserIdAsync(user.Id);
                foreach (var role in roles)
                {
                    allRoles.Add((user.Id, role));
                }
            }

            var assignedLeaderIds = (await _politicalLeaderService.GetAllAsync())
                .Select(l => l.IdentityUserId)
                .ToHashSet();

            var availableLeaders = allUsers
                .Where(u => allRoles.Any(r => r.userId == u.Id && r.role == nameof(Roles.PoliticalLeader))
                        && !assignedLeaderIds.Contains(u.Id))
                .ToList();

            var allParties = await _politicalPartyService.GetAllAsync();
            var activeParties = _mapper.Map<List<PoliticalPartyViewModel>>(allParties).Where(p => p.IsActive).ToList();

            ViewBag.AvailableLeaders = availableLeaders; // List<ApplicationUserDto>
            ViewBag.AvailableParties = activeParties;    // List<PoliticalPartyViewModel>
        }
    }
}

