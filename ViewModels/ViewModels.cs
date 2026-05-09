using AVSSecurityAuditor.Models;

namespace AVSSecurityAuditor.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalRequirements { get; set; }
        public int TotalChapters { get; set; }
        public int TotalAssessments { get; set; }
        public double AverageCompliance { get; set; }
        public List<Assessment> RecentAssessments { get; set; } = new();
        public List<ChapterStatViewModel> ChapterStats { get; set; } = new();
    }

    public class BenchmarkReportViewModel
    {
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string Auditor { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int TotalRequirements { get; set; }
        public int ValidCount { get; set; }
        public int NotValidCount { get; set; }
        public int NotApplicableCount { get; set; }
        public int PendingCount { get; set; }
        public double CompliancePercentage { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string RiskColor { get; set; } = string.Empty;
        public List<ChapterStatViewModel> ChapterStats { get; set; } = new();
        public List<ChapterStatViewModel> WeakAreas { get; set; } = new();
    }

    public class ChapterStatViewModel
    {
        public int ChapterNumber { get; set; }
        public string ChapterName { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Valid { get; set; }
        public int NotValid { get; set; }
        public int NotApplicable { get; set; }
        public int Pending { get; set; }
        public double Compliance { get; set; }
    }

    public class AssessmentDetailViewModel
    {
        public Assessment Assessment { get; set; } = null!;
        public List<AsvsChapter> Chapters { get; set; } = new();
        public Dictionary<int, List<AssessmentItem>> ItemsByChapter { get; set; } = new();
    }

    public class ChecklistViewModel
    {
        public List<AsvsChapter> Chapters { get; set; } = new();
        public int TotalRequirements { get; set; }
        public string? SearchTerm { get; set; }
        public int? FilterChapter { get; set; }
    }

    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
