using eVote470Plus.Core.Application.Interfaces.Political;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminHomeController : Controller
    {
        private readonly IElectionService _electionService;

        public AdminHomeController(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<IActionResult> Index()
        {
            var years = await _electionService.GetElectionYearsAsync();
            ViewBag.ElectionYears = years;
            ViewBag.SelectedYear = years.Any() ? years.Max() : DateTime.Now.Year;
            return View();
        }
    }
}
