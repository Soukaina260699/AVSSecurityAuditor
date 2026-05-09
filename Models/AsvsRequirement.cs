namespace AVSSecurityAuditor.Models
{
    public class AsvsRequirement
    {
        public int Id { get; set; }
        public string RequirementId { get; set; } = string.Empty; // e.g. V1.1.1
        public int ChapterId { get; set; }
        public AsvsChapter? Chapter { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Level1 { get; set; } = string.Empty;
        public string Level2 { get; set; } = string.Empty;
        public string Level3 { get; set; } = string.Empty;
        public string Cwe { get; set; } = string.Empty;
        public string Nist { get; set; } = string.Empty;
        public ICollection<AssessmentItem> AssessmentItems { get; set; } = new List<AssessmentItem>();
    }
}
