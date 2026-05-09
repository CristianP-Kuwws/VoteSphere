using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.ViewModels.Political.Election;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ElectionController : Controller
    {
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public ElectionController(
            IElectionService electionService,
            IMapper mapper)
        {
            _electionService = electionService;
            _mapper = mapper;
        }

        // ─── Index ────────────────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var elections = await _electionService.GetAllAsync();

            // Active election first, then sorted newest to oldest
            var vms = _mapper.Map<List<ElectionViewModel>>(elections)
                .OrderByDescending(e => e.IsActive)
                .ThenByDescending(e => e.ElectionDate)
                .ToList();

            return View(vms);
        }

        // ─── Create ───────────────────────────────────────────────────────────

        public async Task<IActionResult> Create()
        {
            if (await _electionService.HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create an election while another one is active.";
                return RedirectToAction(nameof(Index));
            }

            // Validate all preconditions before showing the form
            var validation = await _electionService.CanCreateElection();
            if (!validation.CanCreate)
            {
                TempData["Error"] = string.Join("<br/>", validation.ValidationErrors);
                return RedirectToAction(nameof(Index));
            }

            return View("Save", new SaveElectionViewModel
            {
                Name = string.Empty,
                ElectionDate = DateTime.Today,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveElectionViewModel vm)
        {
            if (await _electionService.HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create an election while another one is active.";
                return RedirectToAction(nameof(Index));
            }

            // Re-validate preconditions on POST as well
            var validation = await _electionService.CanCreateElection();
            if (!validation.CanCreate)
            {
                validation.ValidationErrors.ForEach(e => ModelState.AddModelError(string.Empty, e));
                return View("Save", vm);
            }

            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<ElectionDto>(vm);
            dto.IsActive = true;
            await _electionService.AddAsync(dto);

            TempData["Success"] = "Election created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Finalize ─────────────────────────────────────────────────────────

        public async Task<IActionResult> Finalize(int id)
        {
            var dto = await _electionService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            if (!dto.IsActive)
            {
                TempData["Error"] = "This election is already finalized.";
                return RedirectToAction(nameof(Index));
            }

            var vm = _mapper.Map<ElectionViewModel>(dto);
            return View(vm); // Finalize.cshtml shows ¿Está seguro? confirmation
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalize(int id, bool confirm)
        {
            var dto = await _electionService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            await _electionService.FinalizeElectionAsync(id);

            TempData["Success"] = "Election finalized successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Results ──────────────────────────────────────────────────────────

        public async Task<IActionResult> Results(int id)
        {
            var dto = await _electionService.GetByIdAsync(id);
            if (dto == null) return RedirectToAction(nameof(Index));

            if (dto.IsActive)
            {
                TempData["Error"] = "Results are only available for finalized elections.";
                return RedirectToAction(nameof(Index));
            }

            var results = await _electionService.GetElectionResultsAsync(id);
            var vms = _mapper.Map<List<ElectionResultViewModel>>(results);

            ViewBag.ElectionName = dto.Name;
            ViewBag.ElectionId = id;
            return View(vms);
        }

        // ─── Electoral Summary (Home) ──────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> GetSummaryByYear(int year)
        {
            var elections = await _electionService.GetElectionsByYearAsync(year);
            var vms = _mapper.Map<List<ElectionViewModel>>(elections);

            ViewBag.SelectedYear = year;
            return PartialView("_ElectoralSummary", vms);
        }

        public async Task<IActionResult> GetElectionYears()
        {
            var years = await _electionService.GetElectionYearsAsync();
            return Json(years);
        }
    }
}
