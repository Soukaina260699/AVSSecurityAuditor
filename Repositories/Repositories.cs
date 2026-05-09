using Microsoft.EntityFrameworkCore;
using AVSSecurityAuditor.Data;
using AVSSecurityAuditor.Interfaces;
using AVSSecurityAuditor.Models;

namespace AVSSecurityAuditor.Repositories
{
    public class AsvsRepository : IAsvsRepository
    {
        private readonly AppDbContext _context;
        public AsvsRepository(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<AsvsChapter>> GetAllChaptersAsync() =>
            await _context.AsvsChapters.Include(c => c.Requirements).OrderBy(c => c.ChapterNumber).ToListAsync();

        public async Task<AsvsChapter?> GetChapterByIdAsync(int id) =>
            await _context.AsvsChapters.Include(c => c.Requirements).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<AsvsRequirement>> GetAllRequirementsAsync() =>
            await _context.AsvsRequirements.Include(r => r.Chapter).OrderBy(r => r.RequirementId).ToListAsync();

        public async Task<IEnumerable<AsvsRequirement>> GetRequirementsByChapterAsync(int chapterId) =>
            await _context.AsvsRequirements.Where(r => r.ChapterId == chapterId).ToListAsync();

        public async Task<AsvsRequirement?> GetRequirementByIdAsync(int id) =>
            await _context.AsvsRequirements.Include(r => r.Chapter).FirstOrDefaultAsync(r => r.Id == id);

        public async Task AddChapterAsync(AsvsChapter chapter) => await _context.AsvsChapters.AddAsync(chapter);
        public async Task AddRequirementAsync(AsvsRequirement requirement) => await _context.AsvsRequirements.AddAsync(requirement);

        public Task UpdateRequirementAsync(AsvsRequirement requirement)
        {
            _context.AsvsRequirements.Update(requirement);
            return Task.CompletedTask;
        }

        public async Task DeleteRequirementAsync(int id)
        {
            var req = await _context.AsvsRequirements.FindAsync(id);
            if (req != null) _context.AsvsRequirements.Remove(req);
        }

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }

    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly AppDbContext _context;
        public AssessmentRepository(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Assessment>> GetUserAssessmentsAsync(string userId) =>
            await _context.Assessments.Where(a => a.UserId == userId).OrderByDescending(a => a.CreatedAt).ToListAsync();

        public async Task<Assessment?> GetAssessmentByIdAsync(int id) =>
            await _context.Assessments.FindAsync(id);

        public async Task<Assessment?> GetAssessmentWithItemsAsync(int id) =>
            await _context.Assessments
                .Include(a => a.Items)
                .ThenInclude(i => i.Requirement)
                .ThenInclude(r => r!.Chapter)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task AddAssessmentAsync(Assessment assessment) => await _context.Assessments.AddAsync(assessment);

        public Task UpdateAssessmentAsync(Assessment assessment)
        {
            _context.Assessments.Update(assessment);
            return Task.CompletedTask;
        }

        public async Task DeleteAssessmentAsync(int id)
        {
            var a = await _context.Assessments.FindAsync(id);
            if (a != null) _context.Assessments.Remove(a);
        }

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}
