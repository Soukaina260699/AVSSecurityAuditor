namespace AVSSecurityAuditor.Models
{
    public class AsvsChapter
    {
        public int Id { get; set; }
        public int ChapterNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<AsvsRequirement> Requirements { get; set; } = new List<AsvsRequirement>();
    }
}
