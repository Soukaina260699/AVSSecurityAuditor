using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AVSSecurityAuditor.Models;

namespace AVSSecurityAuditor.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AsvsChapter> AsvsChapters { get; set; }
        public DbSet<AsvsRequirement> AsvsRequirements { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentItem> AssessmentItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AsvsRequirement>()
                .HasOne(r => r.Chapter)
                .WithMany(c => c.Requirements)
                .HasForeignKey(r => r.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentItem>()
                .HasOne(i => i.Assessment)
                .WithMany(a => a.Items)
                .HasForeignKey(i => i.AssessmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentItem>()
                .HasOne(i => i.Requirement)
                .WithMany(r => r.AssessmentItems)
                .HasForeignKey(i => i.RequirementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Assessment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Assessments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
