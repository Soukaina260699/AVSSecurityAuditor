namespace AVSSecurityAuditor.Models
{
    public enum AssessmentStatus
    {
        Pending,
        Valid,
        NotValid,
        NotApplicable
    }

    public class Assessment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string Auditor { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public ICollection<AssessmentItem> Items { get; set; } = new List<AssessmentItem>();
    }

    public class AssessmentItem
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public Assessment? Assessment { get; set; }
        public int RequirementId { get; set; }
        public AsvsRequirement? Requirement { get; set; }
        public AssessmentStatus Status { get; set; } = AssessmentStatus.Pending;
        public string Notes { get; set; } = string.Empty;
    }
}
