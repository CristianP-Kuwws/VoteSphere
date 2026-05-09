using eVote470Plus.Core.Application.Interfaces.ApplicationUser;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.ViewModels.Login;
using eVote470Plus.Core.Domain.Common.Enum;
using eVote470Plus.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eVote470PlusWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountServicesForWebApp _accountService;
        private readonly IPoliticalLeaderService _politicalLeaderService;

        public LoginController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAccountServicesForWebApp accountService,
            IPoliticalLeaderService politicalLeaderService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _politicalLeaderService = politicalLeaderService;
        }

        public IActionResult Index()
        {
            // If already authenticated redirect to corresponding home
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToCorrespondingHome();

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.FindByNameAsync(vm.UserName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid access credentials.");
                return View(vm);
            }

            // Check active status before attempting sign in
            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "This user is inactive. Please contact an administrator.");
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                vm.Password,
                isPersistent: vm.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid access credentials.");
                return View(vm);
            }

            // If PoliticalLeader, verify they have a party assigned
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(nameof(Roles.PoliticalLeader)))
            {
                var leader = await _politicalLeaderService.GetByIdentityUserIdAsync(user.Id);
                if (leader == null)
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "You have no political party assigned, therefore you cannot log in. Please contact an administrator.");
                    return View(vm);
                }
            }

            return RedirectToCorrespondingHome();
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOut();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // Helpers
        private IActionResult RedirectToCorrespondingHome()
        {
            if (User.IsInRole(nameof(Roles.Administrator)))
                return RedirectToAction("Index", "AdminHome");

            if (User.IsInRole(nameof(Roles.PoliticalLeader)))
                return RedirectToAction("Index", "LeaderHome");

            return RedirectToAction("Index", "Voting");
        }
    }
}