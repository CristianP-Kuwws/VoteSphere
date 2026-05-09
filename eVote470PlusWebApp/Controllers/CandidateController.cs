using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.ViewModels.People.Candidate;
using eVote470PlusWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "PoliticalLeader")]
    public class CandidateController : Controller
    {
        private readonly ICandidateService _candidateService;
        private readonly IElectionService _electionService;
        private readonly IPoliticalLeaderService _politicalLeaderService;
        private readonly ICandidatePositionService _candidatePositionService;
        private readonly IPoliticalPositionService _politicalPositionService;
        private readonly IMapper _mapper;

        public CandidateController(
            ICandidateService candidateService,
            IElectionService electionService,
            IPoliticalLeaderService politicalLeaderService,
            ICandidatePositionService candidatePositionService,
            IPoliticalPositionService politicalPositionService,
            IMapper mapper)
        {
            _candidateService = candidateService;
            _electionService = electionService;
            _politicalLeaderService = politicalLeaderService;
            _candidatePositionService = candidatePositionService;
            _politicalPositionService = politicalPositionService;
            _mapper = mapper;
        }

        private async Task<bool> HasActiveElectionAsync()
            => await _electionService.HasActiveElectionAsync();

        /// <summary>
        /// Gets the party ID of the currently logged-in leader.
        /// </summary>
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

            var candidates = await _candidateService.GetByPoliticalPartyAsync(partyId.Value);
            var vms = _mapper.Map<List<CandidateViewModel>>(candidates);

            // Get all assignments for this party once, avoid N+1
            var assignments = await _candidatePositionService.GetByPartyAsync(partyId.Value);

            foreach (var vm in vms)
            {
                var assignment = assignments.FirstOrDefault(a => a.CandidateId == vm.CandidateId);
                if (assignment != null)
                {
                    var position = await _politicalPositionService.GetByIdAsync(assignment.PoliticalPositionId);
                    if (position != null && position.IsActive)
                        vm.PoliticalPartyPosition = position.Name;
                }
            }

            ViewBag.HasActiveElection = await HasActiveElectionAsync();
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a candidate while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            return View("Save", new SaveCandidateViewModel
            {
                Name = string.Empty,
                LastName = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveCandidateViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a candidate while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentLeaderPartyIdAsync();
            if (partyId == null)
            {
                TempData["Error"] = "You have no political party assigned.";
                return RedirectToAction("Index", "Home");
            }

            if (vm.CandidatePhoto == null)
                ModelState.AddModelError(nameof(vm.CandidatePhoto), "The photo is required.");

            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<CandidateDto>(vm);
            dto.IsActive = true;
            dto.PoliticalPartyId = partyId.Value;

            var created = await _candidateService.AddAsync(dto);

            if (created != null)
            {
                string photoUrl = FileManager.Upload(vm.CandidatePhoto, created.CandidateId, "Candidates");
                created.PhotoUrl = photoUrl;
                await _candidateService.UpdateAsync(created, created.CandidateId);
            }

            TempData["Success"] = "Candidate created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a candidate while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _candidateService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<SaveCandidateViewModel>(dto);
            ViewBag.IsEdit = true;
            ViewBag.CurrentPhoto = dto.PhotoUrl;
            return View("Save", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveCandidateViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a candidate while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = true;
                ViewBag.CurrentPhoto = vm.PhotoUrl;
                return View("Save", vm);
            }

            // Get original to preserve PoliticalPartyId
            var original = await _candidateService.GetByIdAsync(vm.CandidateId);
            if (original == null) return RedirectToAction(nameof(Index));

            var dto = _mapper.Map<CandidateDto>(vm);
            dto.PoliticalPartyId = original.PoliticalPartyId; 

            string photoUrl = FileManager.Upload(vm.CandidatePhoto, vm.CandidateId, "Candidates",
                isEditMode: true, imagePath: vm.PhotoUrl ?? string.Empty);

            dto.PhotoUrl = photoUrl;
            await _candidateService.UpdateAsync(dto, vm.CandidateId);

            TempData["Success"] = "Candidate updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a candidate while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _candidateService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<CandidateViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, bool currentStatus)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a candidate while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _candidateService.ToggleActiveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}