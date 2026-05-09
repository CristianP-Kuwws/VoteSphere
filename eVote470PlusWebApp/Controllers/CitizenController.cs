using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.ViewModels.People.Citizen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CitizenController : Controller
    {
        private readonly ICitizenService _citizenService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public CitizenController(
            ICitizenService citizenService,
            IElectionService electionService,
            IMapper mapper)
        {

            _citizenService = citizenService;
            _electionService = electionService;
            _mapper = mapper;
        }

        private async Task<bool> HasActiveElectionAsync()
            => await _electionService.HasActiveElectionAsync();

        public async Task<IActionResult> Index()
        {
            var citizens = await _citizenService.GetAllAsync();
            var vms = _mapper.Map<List<CitizenViewmodel>>(citizens);

            ViewBag.HasActiveElection = await HasActiveElectionAsync();
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {

            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a citizen while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            return View("Save", new SaveCitizenViewModel
            {
                Name = string.Empty,
                LastName = string.Empty,
                Email = string.Empty,
                DocumentNumber = string.Empty,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveCitizenViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a citizen while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            // excludeId prevents false positive when keeping the same document number
            bool documentTaken = await _citizenService.ExistsDocumentNumberAsync(vm.DocumentNumber);
            if (documentTaken)
            {
                ModelState.AddModelError(nameof(vm.DocumentNumber), "Another citizen already has this document number.");
            }

            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<CitizenDto>(vm);
            dto.IsActive = true; // Ensure new positions are active by default
            await _citizenService.AddAsync(dto);

            TempData["Success"] = "Citizen created successfully.";
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a citizen while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _citizenService.GetByIdAsync(id);

            if (dto == null)
                return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<SaveCitizenViewModel>(dto);
            ViewBag.IsEdit = true;
            return View("Save", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveCitizenViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a citizen while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            // excludeId prevents false positive when keeping the same document number
            bool documentTaken = await _citizenService.ExistsDocumentNumberAsync(vm.DocumentNumber, vm.CitizenId);
            if (documentTaken)
            {
                ModelState.AddModelError(nameof(vm.DocumentNumber), "Another citizen already has this document number.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = true;
                return View("Save", vm);
            }

            var dto = _mapper.Map<CitizenDto>(vm);
            await _citizenService.UpdateAsync(dto, vm.CitizenId);
            TempData["Success"] = "Citizen updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a citizen while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _citizenService.GetByIdAsync(id);
            if (dto == null)
                return RedirectToAction(nameof(Index));


            var vm = _mapper.Map<CitizenViewmodel>(dto);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, bool currentStatus)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a citizen while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _citizenService.ToggleActiveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
