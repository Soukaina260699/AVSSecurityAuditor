using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AVSSecurityAuditor.Interfaces;
using AVSSecurityAuditor.Models;
using AVSSecurityAuditor.ViewModels;

namespace AVSSecurityAuditor.Controllers
{
    public class ChecklistController : Controller
    {
        private readonly IAsvsRepository _asvs;
        public ChecklistController(IAsvsRepository asvs) { _asvs = asvs; }

        public async Task<IActionResult> Index(string? search, int? chapterId)
        {
            var chapters = (await _asvs.GetAllChaptersAsync()).ToList();
            var requirements = (await _asvs.GetAllRequirementsAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                requirements = requirements.Where(r =>
                    r.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    r.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    r.RequirementId.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (chapterId.HasValue)
            {
                requirements = requirements.Where(r => r.ChapterId == chapterId.Value).ToList();
                chapters = chapters.Where(c => c.Id == chapterId.Value).ToList();
            }

            // Rebuild chapters with filtered requirements
            foreach (var chapter in chapters)
                chapter.Requirements = requirements.Where(r => r.ChapterId == chapter.Id).ToList();

            var vm = new ChecklistViewModel
            {
                Chapters = chapters,
                TotalRequirements = requirements.Count,
                SearchTerm = search,
                FilterChapter = chapterId
            };

            return View(vm);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var req = await _asvs.GetRequirementByIdAsync(id);
            if (req == null) return NotFound();
            return View(req);
        }
    }

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signIn;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signIn)
        {
            _userManager = userManager;
            _signIn = signIn;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signIn.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
                return LocalRedirect(returnUrl ?? "/");

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signIn.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();
    }
}
