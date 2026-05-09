using AVSSecurityAuditor.Models;
using AVSSecurityAuditor.ViewModels;

namespace AVSSecurityAuditor.Services
{
    public class ReportService
    {
        public BenchmarkReportViewModel GenerateReport(Assessment assessment)
        {
            var items = assessment.Items.ToList();
            int total = items.Count;
            int valid = items.Count(i => i.Status == AssessmentStatus.Valid);
            int notValid = items.Count(i => i.Status == AssessmentStatus.NotValid);
            int notApplicable = items.Count(i => i.Status == AssessmentStatus.NotApplicable);
            int pending = items.Count(i => i.Status == AssessmentStatus.Pending);

            int applicable = total - notApplicable;
            double compliance = applicable > 0 ? Math.Round((double)valid / applicable * 100, 1) : 0;

            string riskLevel = compliance >= 80 ? "Low" : compliance >= 50 ? "Medium" : "High";
            string riskColor = compliance >= 80 ? "success" : compliance >= 50 ? "warning" : "danger";

            var chapterStats = items
                .Where(i => i.Requirement?.Chapter != null)
                .GroupBy(i => i.Requirement!.Chapter!)
                .Select(g => new ChapterStatViewModel
                {
                    ChapterNumber = g.Key.ChapterNumber,
                    ChapterName = g.Key.Name,
                    Total = g.Count(),
                    Valid = g.Count(i => i.Status == AssessmentStatus.Valid),
                    NotValid = g.Count(i => i.Status == AssessmentStatus.NotValid),
                    NotApplicable = g.Count(i => i.Status == AssessmentStatus.NotApplicable),
                    Pending = g.Count(i => i.Status == AssessmentStatus.Pending),
                    Compliance = g.Count() - g.Count(i => i.Status == AssessmentStatus.NotApplicable) > 0
                        ? Math.Round((double)g.Count(i => i.Status == AssessmentStatus.Valid) /
                          (g.Count() - g.Count(i => i.Status == AssessmentStatus.NotApplicable)) * 100, 1)
                        : 0
                })
                .OrderBy(c => c.ChapterNumber)
                .ToList();

            var weakAreas = chapterStats.Where(c => c.Compliance < 50).OrderBy(c => c.Compliance).ToList();

            return new BenchmarkReportViewModel
            {
                AssessmentId = assessment.Id,
                AssessmentName = assessment.Name,
                ProjectName = assessment.ProjectName,
                Auditor = assessment.Auditor,
                CreatedAt = assessment.CreatedAt,
                TotalRequirements = total,
                ValidCount = valid,
                NotValidCount = notValid,
                NotApplicableCount = notApplicable,
                PendingCount = pending,
                CompliancePercentage = compliance,
                RiskLevel = riskLevel,
                RiskColor = riskColor,
                ChapterStats = chapterStats,
                WeakAreas = weakAreas
            };
        }
    }
}
