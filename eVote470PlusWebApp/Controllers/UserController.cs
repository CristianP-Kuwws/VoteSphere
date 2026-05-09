using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.ApplicationUser;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.ViewModels.People.ApplicationUser;
using eVote470Plus.Core.Domain.Common.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly IAccountServicesForWebApp _accountServicesForWebApp;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public UserController(
            IAccountServicesForWebApp accountServicesForWebApp,
            IElectionService electionService,
            IMapper mapper)
        {
            _accountServicesForWebApp = accountServicesForWebApp;
            _electionService = electionService;
            _mapper = mapper;
        }

        private async Task<bool> HasActiveElectionAsync()
            => await _electionService.HasActiveElectionAsync();

        public async Task<IActionResult> Index()
        {
            var users = await _accountServicesForWebApp.GetAllAsync(null);
            var vms = _mapper.Map<List<ApplicationUserViewModel>>(users);

            foreach (var vm in vms)
            {
                var roles = await _accountServicesForWebApp.GetRolesByUserIdAsync(vm.Id);
                vm.Role = roles.FirstOrDefault() ?? "No Role";
            }

            ViewBag.HasActiveElection = await HasActiveElectionAsync();

            return View(vms);
        }

        public async Task<IActionResult> Create()
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a user while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CreateApplicationUserViewModel
            {
                Name = string.Empty,
                LastName = string.Empty,
                Email = string.Empty,
                UserName = string.Empty,
                Password = string.Empty,
                ConfirmPassword = string.Empty,
                IsActive = true,
                Role = nameof(Roles.Administrator)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateApplicationUserViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot create a user while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View(vm);

            var dto = _mapper.Map<CreateApplicationUserDto>(vm);
            var result = await _accountServicesForWebApp.CreateUserAsync(dto);

            if (result.HasError)
            {
                result.Errors.ForEach(error =>
                    ModelState.AddModelError(string.Empty, error));

                return View(vm);
            }

            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a user while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _accountServicesForWebApp.GetUserById(id);

            if (dto == null)
                return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<EditApplicationUserViewModel>(dto);

            var roles = await _accountServicesForWebApp.GetRolesByUserIdAsync(id);

            vm.Role = roles.FirstOrDefault() ?? nameof(Roles.Administrator);
            vm.Password = null;
            vm.ConfirmPassword = null;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditApplicationUserViewModel vm)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot edit a user while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                vm.Password = null;
                vm.ConfirmPassword = null;
            }
            else if (vm.Password != vm.ConfirmPassword)
            {
                ModelState.AddModelError(
                    nameof(vm.ConfirmPassword),
                    "Passwords don't match.");
            }

            if (!ModelState.IsValid)
                return View(vm);

            var dto = _mapper.Map<EditApplicationUserDto>(vm);

            var result = await _accountServicesForWebApp.EditUserAsync(dto);

            if (result.HasError)
            {
                result.Errors.ForEach(error =>
                    ModelState.AddModelError(string.Empty, error));

                return View(vm);
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a user while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _accountServicesForWebApp.GetUserById(id);

            if (dto == null)
                return RedirectToAction(nameof(Index));

            var vm = _mapper.Map<ApplicationUserViewModel>(dto);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string id, bool currentStatus)
        {
            if (await HasActiveElectionAsync())
            {
                TempData["Error"] = "Cannot change the status of a user while an election is active.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _accountServicesForWebApp.ToggleActiveAsync(id);

            if (result.HasError)
            {
                TempData["Error"] = string.Join(", ", result.Errors!);
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "User status updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}