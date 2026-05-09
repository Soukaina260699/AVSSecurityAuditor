using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AVSSecurityAuditor.Models;
using AVSSecurityAuditor.Interfaces;
using AVSSecurityAuditor.ViewModels;
using Microsoft.EntityFrameworkCore;
using AVSSecurityAuditor.Data;

namespace AVSSecurityAuditor.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAsvsRepository _asvs;
        private readonly IAssessmentRepository _assessment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public HomeController(IAsvsRepository asvs, IAssessmentRepository assessment,
            UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _asvs = asvs;
            _assessment = assessment;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var chapters = await _asvs.GetAllChaptersAsync();
            var requirements = await _asvs.GetAllRequirementsAsync();

            var vm = new DashboardViewModel
            {
                TotalRequirements = requirements.Count(),
                TotalChapters = chapters.Count(),
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var assessments = await _assessment.GetUserAssessmentsAsync(user.Id);
                    vm.TotalAssessments = assessments.Count();
                    vm.RecentAssessments = assessments.Take(5).ToList();

                    // Calculate average compliance
                    var compliances = new List<double>();
                    foreach (var a in assessments.Take(5))
                    {
                        var full = await _assessment.GetAssessmentWithItemsAsync(a.Id);
                        if (full?.Items.Any() == true)
                        {
                            var applicable = full.Items.Count(i => i.Status != AssessmentStatus.NotApplicable);
                            if (applicable > 0)
                            {
                                var valid = full.Items.Count(i => i.Status == AssessmentStatus.Valid);
                                compliances.Add((double)valid / applicable * 100);
                            }
                        }
                    }
                    vm.AverageCompliance = compliances.Any() ? Math.Round(compliances.Average(), 1) : 0;
                }
            }

            return View(vm);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
