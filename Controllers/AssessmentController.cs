using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AVSSecurityAuditor.Interfaces;
using AVSSecurityAuditor.Models;
using AVSSecurityAuditor.Services;
using AVSSecurityAuditor.ViewModels;

namespace AVSSecurityAuditor.Controllers
{
    [Authorize]
    public class AssessmentController : Controller
    {
        private readonly IAssessmentRepository _assessment;
        private readonly IAsvsRepository _asvs;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ReportService _report;
        private readonly AiAssistantService _ai;

        public AssessmentController(IAssessmentRepository assessment, IAsvsRepository asvs,
            UserManager<ApplicationUser> userManager, ReportService report, AiAssistantService ai)
        {
            _assessment = assessment;
            _asvs = asvs;
            _userManager = userManager;
            _report = report;
            _ai = ai;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var assessments = await _assessment.GetUserAssessmentsAsync(user!.Id);
            return View(assessments.ToList());
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name, string projectName, string auditor)
        {
            var user = await _userManager.GetUserAsync(User);
            var requirements = await _asvs.GetAllRequirementsAsync();

            var assessment = new Assessment
            {
                Name = name,
                ProjectName = projectName,
                Auditor = auditor,
                UserId = user!.Id,
                CreatedAt = DateTime.UtcNow
            };

            // Pre-populate all requirements as Pending
            assessment.Items = requirements.Select(r => new AssessmentItem
            {
                RequirementId = r.Id,
                Status = AssessmentStatus.Pending,
                Notes = string.Empty
            }).ToList();

            await _assessment.AddAssessmentAsync(assessment);
            await _assessment.SaveChangesAsync();

            TempData["Success"] = "Assessment created successfully!";
            return RedirectToAction("Evaluate", new { id = assessment.Id });
        }

        public async Task<IActionResult> Evaluate(int id, int? chapterId)
        {
            var assessment = await _assessment.GetAssessmentWithItemsAsync(id);
            if (assessment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (assessment.UserId != user!.Id && !User.IsInRole("Admin"))
                return Forbid();

            var chapters = (await _asvs.GetAllChaptersAsync()).ToList();
            var itemsByChapter = assessment.Items
                .Where(i => i.Requirement?.Chapter != null)
                .GroupBy(i => i.Requirement!.ChapterId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var vm = new AssessmentDetailViewModel
            {
                Assessment = assessment,
                Chapters = chapters,
                ItemsByChapter = itemsByChapter
            };

            if (chapterId.HasValue) ViewBag.ActiveChapter = chapterId.Value;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem(int assessmentId, int itemId, AssessmentStatus status, string notes)
        {
            var assessment = await _assessment.GetAssessmentWithItemsAsync(assessmentId);
            if (assessment == null) return NotFound();

            var item = assessment.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Status = status;
                item.Notes = notes ?? string.Empty;
                assessment.UpdatedAt = DateTime.UtcNow;
                await _assessment.UpdateAssessmentAsync(assessment);
                await _assessment.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpdate(int assessmentId, int chapterId, AssessmentStatus status)
        {
            var assessment = await _assessment.GetAssessmentWithItemsAsync(assessmentId);
            if (assessment == null) return NotFound();

            foreach (var item in assessment.Items.Where(i => i.Requirement?.ChapterId == chapterId))
                item.Status = status;

            assessment.UpdatedAt = DateTime.UtcNow;
            await _assessment.UpdateAssessmentAsync(assessment);
            await _assessment.SaveChangesAsync();

            TempData["Success"] = "Chapter updated successfully.";
            return RedirectToAction("Evaluate", new { id = assessmentId, chapterId });
        }

        public async Task<IActionResult> Report(int id)
        {
            var assessment = await _assessment.GetAssessmentWithItemsAsync(id);
            if (assessment == null) return NotFound();

            var vm = _report.GenerateReport(assessment);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AiExplain(int requirementId, string lang = "en")
        {
            var req = await _asvs.GetRequirementByIdAsync(requirementId);
            if (req == null) return NotFound();

            var explanation = await _ai.ExplainRequirementAsync(req.RequirementId, req.Description, lang);
            return Json(new { explanation, requirementId = req.RequirementId, title = req.Title });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var assessment = await _assessment.GetAssessmentByIdAsync(id);
            if (assessment == null || (assessment.UserId != user!.Id && !User.IsInRole("Admin")))
                return Forbid();

            await _assessment.DeleteAssessmentAsync(id);
            await _assessment.SaveChangesAsync();
            TempData["Success"] = "Assessment deleted.";
            return RedirectToAction("Index");
        }
    }
}
