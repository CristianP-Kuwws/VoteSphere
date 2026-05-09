using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.ViewModels.Political.PoliticalParty;
using eVote470PlusWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PoliticalPartyController : Controller
    {
        private readonly IPoliticalPartyService _politicalPartyService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public PoliticalPartyController(
            IPoliticalPartyService politicalPartyService,
            IElectionService electionService,
            IMapper mapper)
        {

            _politicalPartyService = politicalPartyService;
            _electionService = electionService;
            _mapper = mapper;
        }

        private async Task<bool> HasActiveElectionAsync()
            => await _electionService.HasActiveElectionAsync();

        public async Task<IActionResult> Index()
        {
            var parties = await _politicalPartyService.GetAllAsync();
            var vms = _mapper.Map<List<PoliticalPartyViewModel>>(parties);

            ViewBag.HasActiveElection = await HasActiveElectionAsync();
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {

            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a political party while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            return View("Save", new SavePoliticalPartyViewModel
            {
                Name = string.Empty,
                Acronym = string.Empty,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePoliticalPartyViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a political party while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            // excludeId prevents false positive when keeping the same document number
            bool acronymTaken = await _politicalPartyService.ExistAcronymAsync(vm.Acronym);
            if (acronymTaken)
                ModelState.AddModelError(nameof(vm.Acronym), "A political party with this acronym already exists.");

            if (vm.LogoImg == null)
                ModelState.AddModelError(nameof(vm.LogoImg), "The Logo field is required.");

            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<PoliticalPartyDto>(vm);
            dto.IsActive = true; // Ensure new parties are active by default

            // Add with placeholder id=0 first to get real id for folder naming
            var created = await _politicalPartyService.AddAsync(dto);

            if (created != null)
            {
                string logoUrl = FileManager.Upload(vm.LogoImg!, created.PoliticalPartyId, "PoliticalParties");
                created.LogoUrl = logoUrl;
                await _politicalPartyService.UpdateAsync(created, created.PoliticalPartyId);
            }

            TempData["Success"] = "Political party created successfully.";
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a political party while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _politicalPartyService.GetByIdAsync(id);

            if (dto == null)
                return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<SavePoliticalPartyViewModel>(dto);
            ViewBag.IsEdit = true;
            ViewBag.CurrentLogo = dto.LogoUrl; // so the view can show the existing logo
            return View("Save", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePoliticalPartyViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a political party while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            // excludeId prevents false positive when keeping the same document number
            bool acronymTaken = await _politicalPartyService.ExistAcronymAsync(vm.Acronym, vm.PoliticalPartyId);
            if (acronymTaken)
                ModelState.AddModelError(nameof(vm.Acronym), "A political party with this acronym already exists.");

            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = true;
                ViewBag.CurrentLogo = vm.LogoUrl; // preserve preview on validation failure
                return View("Save", vm);
            }

            var dto = _mapper.Map<PoliticalPartyDto>(vm);

            string logoUrl = FileManager.Upload(vm.LogoImg, vm.PoliticalPartyId, "PoliticalParties", 
                isEditMode: true, imagePath: vm.LogoUrl ?? string.Empty);

            dto.LogoUrl = logoUrl;

            await _politicalPartyService.UpdateAsync(dto, vm.PoliticalPartyId);

            TempData["Success"] = "Political party updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a political party while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _politicalPartyService.GetByIdAsync(id);
            if (dto == null)
                return RedirectToAction(nameof(Index));


            var vm = _mapper.Map<PoliticalPartyViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, bool currentStatus)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a political party while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _politicalPartyService.ToggleActiveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
