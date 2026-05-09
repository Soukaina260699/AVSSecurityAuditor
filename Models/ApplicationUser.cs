using Microsoft.AspNetCore.Identity;

namespace AVSSecurityAuditor.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    }
}
