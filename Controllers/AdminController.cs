using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AVSSecurityAuditor.Interfaces;
using AVSSecurityAuditor.Models;
using AVSSecurityAuditor.Services;

namespace AVSSecurityAuditor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAsvsRepository _asvs;
        private readonly CsvImportService _csv;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAsvsRepository asvs, CsvImportService csv, ILogger<AdminController> logger)
        {
            _asvs = asvs;
            _csv = csv;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var chapters = await _asvs.GetAllChaptersAsync();
            var requirements = await _asvs.GetAllRequirementsAsync();
            ViewBag.ChapterCount = chapters.Count();
            ViewBag.RequirementCount = requirements.Count();
            return View();
        }

        [HttpGet]
        public IActionResult Import() => View();

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Error"] = "Please select a valid CSV file.";
                return RedirectToAction("Import");
            }

            try
            {
                using var stream = csvFile.OpenReadStream();
                var (chapters, reqs) = await _csv.ImportAsync(stream);
                TempData["Success"] = $"✅ Import successful: {chapters} chapters, {reqs} requirements imported.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CSV import failed");
                TempData["Error"] = $"Import failed: {ex.Message}";
            }

            return RedirectToAction("Import");
        }

        public async Task<IActionResult> Requirements(int? chapterId, string? search)
        {
            IEnumerable<AsvsRequirement> reqs;
            if (chapterId.HasValue)
                reqs = await _asvs.GetRequirementsByChapterAsync(chapterId.Value);
            else
                reqs = await _asvs.GetAllRequirementsAsync();

            if (!string.IsNullOrWhiteSpace(search))
                reqs = reqs.Where(r => r.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || r.RequirementId.Contains(search, StringComparison.OrdinalIgnoreCase));

            ViewBag.Chapters = await _asvs.GetAllChaptersAsync();
            ViewBag.Search = search;
            ViewBag.ChapterId = chapterId;
            return View(reqs.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> EditRequirement(int id)
        {
            var req = await _asvs.GetRequirementByIdAsync(id);
            if (req == null) return NotFound();
            return View(req);
        }

        [HttpPost]
        public async Task<IActionResult> EditRequirement(AsvsRequirement model)
        {
            if (!ModelState.IsValid) return View(model);
            await _asvs.UpdateRequirementAsync(model);
            await _asvs.SaveChangesAsync();
            TempData["Success"] = "Requirement updated successfully.";
            return RedirectToAction("Requirements");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRequirement(int id)
        {
            await _asvs.DeleteRequirementAsync(id);
            await _asvs.SaveChangesAsync();
            TempData["Success"] = "Requirement deleted.";
            return RedirectToAction("Requirements");
        }

        public async Task<IActionResult> Chapters()
        {
            var chapters = await _asvs.GetAllChaptersAsync();
            return View(chapters.ToList());
        }
    }
}
