using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.ViewModels.Political.PoliticalPosition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PoliticalPositionController : Controller
    {
        private readonly IPoliticalPositionService _politicalPositionService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public PoliticalPositionController(
            IPoliticalPositionService politicalPositionService,
            IElectionService electionService,
            IMapper mapper)
        {

            _politicalPositionService = politicalPositionService;
            _electionService = electionService;
            _mapper = mapper;
        }

        private async Task<bool> HasActiveElectionAsync()
            => await _electionService.HasActiveElectionAsync();

        public async Task<IActionResult> Index()
        {
            var positions = await _politicalPositionService.GetAllAsync();
            var vms = _mapper.Map<List<PoliticalPositionViewModel>>(positions);

            ViewBag.HasActiveElection = await HasActiveElectionAsync();
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {

            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a political position while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            return View("Save", new SavePoliticalPositionViewModel
            {
                Name = string.Empty,
                Description = string.Empty,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePoliticalPositionViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a political position while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<PoliticalPositionDto>(vm);
            dto.IsActive = true; // Ensure new positions are active by default
            await _politicalPositionService.AddAsync(dto);

            TempData["Success"] = "Political position created successfully.";
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a political position while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _politicalPositionService.GetByIdAsync(id);

            if (dto == null)
                return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<SavePoliticalPositionViewModel>(dto);
            ViewBag.IsEdit = true;
            return View("Save", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePoliticalPositionViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a political position while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = true;
                return View("Save", vm);
            }

            var dto = _mapper.Map<PoliticalPositionDto>(vm);
            await _politicalPositionService.UpdateAsync(dto, vm.PoliticalPositionId);
            TempData["Success"] = "Political position updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a political position while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _politicalPositionService.GetByIdAsync(id);
            if (dto == null)
                return RedirectToAction(nameof(Index));


            var vm = _mapper.Map<PoliticalPositionViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, bool currentStatus)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a political position while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            await _politicalPositionService.ToggleActiveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
